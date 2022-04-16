using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moves
{
    //The player can move,so they will inherit all of the main traits that a moving character has.
    private GameManager manager;
    public int killCount;
    private bool gameEnded;
    protected override void Awake()
    {
        base.Awake();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    protected override void Start()
    {
        base.Start();
        moveSpeed = 3f;
        killCount = 0;
        gameEnded = false;
    }
    protected override IEnumerator ShootGun()
    {
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
        manager.UpdateUIBars();
        yield return new WaitForSeconds(.125f);

        attackingState = 0;
    }
    protected override IEnumerator ThrowShiruken()
    {
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
        manager.UpdateUIBars();
        yield return new WaitForSeconds(.125f);

        attackingState = 0; 
    }
    protected override void OnCollisionEnter(Collision coll)
    {
        if (invincible)
            return;
        if (coll.collider.tag == "Win")
            manager.PlayerWins();
        if (coll.collider.tag == "Killbox")
        {
            GameManager.instance.GameOver();
            Destroy(gameObject);
        }
        if (coll.collider.gameObject.tag == "Fighter")
        {
            CheckAttackState(attackingState);

            Damage dmg = new Damage
            {
                origin = transform.position,
                damage = attackDamage,
                knockback = knockbackMultipler
            };

            coll.collider.gameObject.GetComponentInParent<Enemy>().SendMessage("TakeDamage", dmg);
        }
        if (coll.collider.name.Contains("Bullet"))
            TakeDamage(new Damage { damage = 2, knockback = 0, origin = Vector3.zero });
        if (coll.collider.name.Contains("Shiruken"))
            TakeDamage(new Damage { damage = 1, knockback = 0, origin = Vector3.zero });
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            switch (other.gameObject.GetComponent<Pickup>().pickupType)
            {
                case 0:
                    HealActor(other.gameObject.GetComponent<Pickup>().amount);
                    break;
                case 1:
                    GiveGunAmmo(other.GetComponent<Pickup>().amount);
                    break;
                case 2:
                    GiveShirukenAmmo(other.gameObject.GetComponent<Pickup>().amount);
                    break;
                default:
                    Debug.LogError("Default reached on OnCollisionEnter.switch!");
                    break;
            }
            return;
        }
    }
    protected override void HealActor(int amount)
    {
        base.HealActor(amount);
        manager.UpdateUIBars();
    }
    protected override void GiveGunAmmo(int amount)
    {
        base.GiveGunAmmo(amount);
        manager.UpdateUIBars();
    }
    protected override void GiveShirukenAmmo(int amount)
    {
        base.GiveShirukenAmmo(amount);
        manager.UpdateUIBars();
    }
    protected override void TakeDamage(Damage dmg)
    {
        base.TakeDamage(dmg);
        manager.UpdateUIBars();
    }
    public void OnWin()
    {
        gameEnded = true;
        isAlive = false;
        actorAnimator.SetBool("IsMoving", false);
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        GameManager.instance.GameOver();
        isAlive = false;
        gameEnded = true;
        StartCoroutine(DeathCoroutine());
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (dashCooldownTimer < 5 || rollCooldownTimer < 2)
            manager.UpdateUIBars();

        float movement = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.LeftShift))
            movement *= 2;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            movement /= 2;

        if (isAlive)
            MoveActor(new Vector3(movement, 0, 0));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded)
            manager.TogglePause();

        if (!isAlive || manager.paused)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || canDoubleJump))
            Jump();
        if(Input.GetKeyDown(KeyCode.D))
        {
            dashTimer = .25f;
            if (!lastMovedRight)
            {
                lastMovedRight = true;
            }
            else if (lastMovedRight && canDash && canMove)
                StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            dashTimer = .25f;
            if (lastMovedRight)
            {
                lastMovedRight = false;
            }
            else if (!lastMovedRight && canDash && canMove)
                StartCoroutine(Dash());
        }
        if (Input.GetKeyDown(KeyCode.C) && canRoll)
            StartCoroutine(Roll());
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShootBullet && currentGunAmmo > 0)
            StartCoroutine(ShootGun());
        if (Input.GetKeyDown(KeyCode.Mouse1) && canThrowShiruken && currentshirukenAmmo > 0)
            StartCoroutine(ThrowShiruken());
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Punch());
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isGrounded)
                FlyingKick();
            else
                StartCoroutine(Kick());
        }
        if (Input.GetKeyDown(KeyCode.F) && isGrounded)
            StartCoroutine(Uppercut());

    }
}