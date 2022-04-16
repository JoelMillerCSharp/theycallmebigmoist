using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Moves
{
    //Variables for initiating enemy chasing.
    [SerializeField]
    protected float chaseDistance;
    [SerializeField]
    protected float meleeDistance;
    [SerializeField]
    protected bool isChasing;
    [SerializeField]
    protected float distanceFromPlayer;
    protected bool touchingPlayer;
    protected Vector3 playerPosition;
    protected Vector3 currentPosition;

    protected override void Start()
    {
        base.Start();
        isChasing = false;
        chaseDistance = 10f;
        meleeDistance = 3;
    }
    protected override void OnCollisionEnter(Collision coll)
    {
        if (invincible)
            return;
        if (coll.collider.tag == "Killbox")
            OnDeath();
        if (coll.collider.gameObject.tag == "Player")
        {
            CheckAttackState(attackingState);

            Damage dmg = new Damage
            {
                origin = transform.position,
                damage = attackDamage,
                knockback = knockbackMultipler
            };

            coll.collider.gameObject.GetComponentInParent<Player>().SendMessage("TakeDamage", dmg);
        }
        if (coll.collider.name.Contains("Bullet"))
            TakeDamage(new Damage { damage = 2, knockback = 0, origin = Vector3.zero });
        if (coll.collider.name.Contains("Shiruken"))
            TakeDamage(new Damage { damage = 1, knockback = 0, origin = Vector3.zero });
    }
    protected virtual IEnumerator InitiateFlyingKick()
    {
        Jump();

        yield return new WaitForSeconds(.5f);

        FlyingKick();
    }
    protected override IEnumerator ShootGun()
    {
        if (!hasGun)
            yield break;

        timeLastImmune = Time.time;
        audioHandler.PunchSound();

        if (facingRight)
        {
            GameObject bulletProps = Instantiate(bullet, gun.transform.position + new Vector3(.2f, 0, -.5f), transform.rotation);
            actorAnimator.SetTrigger("shoot");
            attackingState = 1;
            bulletProps.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            GameObject bulletProps = Instantiate(bullet, gun.transform.position + new Vector3(-.2f, 0, -.5f), transform.rotation);
            actorAnimator.SetTrigger("shoot");
            attackingState = 1;
            bulletProps.transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        currentGunAmmo--;
        yield return new WaitForSeconds(.125f);

        attackingState = 0;
    }
    protected override IEnumerator ThrowShiruken()
    {
        if (!hasShiruken)
            yield break;

        timeLastImmune = Time.time;
        audioHandler.PunchSound();

        if (facingRight)
        {
            GameObject shirProps = Instantiate(shiruken, transform.position + new Vector3(.75f, 0, 0), gameObject.transform.rotation);
            shirProps.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            GameObject shirProps = Instantiate(shiruken, transform.position + new Vector3(-.75f, 0, 0), gameObject.transform.rotation);
            shirProps.transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        attackingState = 2;
        actorAnimator.SetTrigger("star");
        currentshirukenAmmo--;
        yield return new WaitForSeconds(.125f);

        attackingState = 0;
    }
    protected virtual IEnumerator PickMeleeAttack(int attackNum)
    {
        attacking = true;
        switch (attackNum)
        {
            case 0:
                goto default;
            case 1:
                StartCoroutine(Punch());
                break;
            case 2:
                StartCoroutine(Kick());
                break;
            case 3:
                StartCoroutine(Uppercut());
                break;
            case 4:
                StartCoroutine(InitiateFlyingKick());
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(1.5f);
        attacking = false;
    }
    protected virtual IEnumerator TryRangedAttack()
    {
        attacking = true;

        //Check if there isn't another fight in front of this one in order in shoot
        Ray actorRay = new Ray();
        actorRay.origin = transform.position;
        actorRay.direction = transform.forward;
        if (Physics.Raycast(actorRay, out RaycastHit rHit, distanceFromPlayer))
            if (rHit.collider.tag != "Player")
                yield break;

        //Pick between shooting the gun and throwing a shuriken
        int attack = Random.Range(0, 100);
        if (attack > 69)
            StartCoroutine(ShootGun());
        else if (attack > 39)
            StartCoroutine(ThrowShiruken());
        yield return new WaitForSeconds(1.5f);

        attacking = false;
    }
    protected override void OnDeath()
    {
        if (!isAlive)
            return;
        GameManager.instance.player.killCount++;
        GameManager.instance.UpdateUIBars();
        isAlive = false;
        StartCoroutine(DeathCoroutine());
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //Checks if the player is in range before chasing
        currentPosition = transform.position;
        distanceFromPlayer = Vector3.Distance(playerPosition, currentPosition);
        playerPosition = GameManager.instance.player.transform.position;

        if (isChasing)
        {
            playerPosition = GameManager.instance.player.transform.position;
            MoveActor((playerPosition - transform.position).normalized);

            if(!attacking)
            {
                if (distanceFromPlayer > meleeDistance)
                    StartCoroutine(TryRangedAttack());
                else
                    StartCoroutine(PickMeleeAttack(Random.Range(0, 5)));
            }
            return;
        }
        if (distanceFromPlayer < chaseDistance)
            isChasing = true;
    }
}