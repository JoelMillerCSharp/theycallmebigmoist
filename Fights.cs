using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles health and damage for all actors capable of dying.
public class Fights : Collidable
{
    //Health Variables
    public int maxHealth;
    public int currentHealth;
    protected bool isAlive = true;

    //Time Immune to Damage
    protected float immuneTimer = 1.5f;
    protected float timeLastImmune;
    protected bool invincible = false;

    //Local Damage Variables (Used for instantiating a new Damage Struct)
    protected int attackDamage;
    protected float knockbackMultipler;

    //Knockback Variables
    protected Vector3 knockbackDirection;
    protected float knockbackRecoverySpeed;

    //Ammo Variables
    [SerializeField]
    protected bool hasGun;
    [SerializeField]
    protected bool hasShiruken;
    public int maxGunAmmo;
    public int currentGunAmmo;
    public int maxShirukenAmmo;
    public int currentshirukenAmmo;
    [SerializeField]
    protected GameObject bullet;
    [SerializeField]
    protected GameObject shiruken;
    protected bool canShootBullet = true;
    protected bool canThrowShiruken = true;
    protected bool facingRight = true;

    //Animation and Collision Variables
    [SerializeField]
    protected Animator actorAnimator;
    [SerializeField]
    protected GameObject gun;
    [SerializeField]
    protected bool enemy;
    protected bool attacking = false;

    //Audio Stuff
    [SerializeField]
    protected VolumeSlider audioHandler;

    /// <summary>
    /// Attacking State:
    /// 0 = Not Attacking
    /// 1 = Shooting
    /// 2 = Throwing Shiruken
    /// 3 = Punching
    /// 4 = Kicking
    /// 5 = Uppercut
    /// 6 = Flying Kick
    /// </summary>
    [SerializeField]
    protected int attackingState = 0;

    protected virtual void Awake()
    {
        actorAnimator = transform.GetChild(0).GetComponent<Animator>();
        audioHandler = GameObject.Find("GameManager").GetComponent<VolumeSlider>();
    }
    protected override void Start()
    {
        base.Start();

        attackingState = 0;
        attacking = false;
        immuneTimer = .125f;
        invincible = false;
        canShootBullet = true;
        canThrowShiruken = true;
        facingRight = true;
        currentHealth = maxHealth;
        knockbackRecoverySpeed = 2;
        audioHandler = GameObject.Find("GameManager").GetComponent<VolumeSlider>();
    }
    //Fighting Moves
    protected virtual IEnumerator ShootGun()
    {
        if (!hasGun)
            yield break;
        
        timeLastImmune = Time.time;

        if (facingRight)
        {
            GameObject bulletProps = Instantiate(bullet, gun.transform.position + new Vector3(.5f, 0, .5f), transform.rotation);
            actorAnimator.SetTrigger("shoot");
            attackingState = 1;
            bulletProps.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            GameObject bulletProps = Instantiate(bullet, gun.transform.position + new Vector3(-.5f, 0, .5f), transform.rotation);
            actorAnimator.SetTrigger("shoot");
            attackingState = 1;
            bulletProps.transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        currentGunAmmo--;

        yield return new WaitForSeconds(.125f);

        attackingState = 0;
    }
    protected virtual IEnumerator ThrowShiruken()
    {
        if (!hasShiruken)
            yield break;
        timeLastImmune = Time.time;

        if (facingRight)
        {
            GameObject shirProps = Instantiate(shiruken, transform.position + new Vector3(1.5f, 0, .5f), gameObject.transform.rotation);
            actorAnimator.SetTrigger("star");
            attackingState = 2;
            shirProps.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            GameObject shirProps = Instantiate(shiruken, transform.position + new Vector3(-1.5f, 0, .5f), gameObject.transform.rotation);
            actorAnimator.SetTrigger("star");
            attackingState = 2;
            shirProps.transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        currentshirukenAmmo--;

        yield return new WaitForSeconds(.125f);

        attackingState = 0;
    }
    protected virtual IEnumerator Punch()
    {
        attackingState = 3;
        actorAnimator.SetTrigger("punch");
        audioHandler.PunchSound();

        yield return new WaitForSeconds(.125f);
        attackingState = 0;
    }
    protected virtual IEnumerator Kick()
    {
        attackingState = 4;
        actorAnimator.SetTrigger("kick");
        audioHandler.KickSound();

        yield return new WaitForSeconds(.125f);
        attackingState = 0;
    }
    protected virtual IEnumerator Uppercut()
    {
        attackingState = 5;
        actorAnimator.SetTrigger("uppercut");
        audioHandler.PunchSound();

        yield return new WaitForSeconds(.5f);
        attackingState = 0;
    }
    protected virtual void FlyingKick()
    {
        attackingState = 6;
        actorAnimator.SetTrigger("fkick");
        audioHandler.KickSound();
    }

    //Checks the collider to see what's hit it, as well as what kind of damage to apply. Used for detecting specific attacks.
    protected virtual void CheckAttackState(int attackState)
    {
        switch(attackState)
        {
            //Case 0-2 don't use melee attacks, so all of their values are the same: 0 damage, 0 knockback.
            case 0:
            case 1:
            case 2:
                {
                    attackDamage = 0;
                    knockbackMultipler = 0;
                }
                break;
            //Punch
            case 3:
                {
                    attackDamage = 3;
                    knockbackMultipler = 150f;
                }
                break;
            //Kick
            case 4:
                {
                    attackDamage = 3;
                    knockbackMultipler = 150;
                }
                break;
            //Uppercut
            case 5:
                {
                    attackDamage = 4;
                    knockbackMultipler = 250;
                }
                break;
            //Flying Kick
            case 6:
                {
                    attackDamage = 4;
                    knockbackMultipler = 250;
                }
                break;
            default:
                break;
        }

    }

    //Casts an instance of damage onto the affected actor.
    protected virtual void TakeDamage(Damage dmg)
    {
        if(Time.time - timeLastImmune > immuneTimer)
        {
            timeLastImmune = Time.time;
            currentHealth -= dmg.damage;
            knockbackDirection = new Vector3(transform.position.x - dmg.origin.x, transform.position.y, transform.position.z).normalized * dmg.knockback;
        }
        Debug.Log(dmg.damage + " damage dealt to " + gameObject.name);
        if (currentHealth <= 0)
            OnDeath();
    }
    protected virtual void HealActor(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log(gameObject.name + " healed for " + amount + " health!");
    }
    protected virtual void GiveGunAmmo(int amount)
    {
        currentGunAmmo += amount;

        if (currentGunAmmo >= maxGunAmmo)
            currentGunAmmo = maxGunAmmo;
        Debug.Log(gameObject.name + " was given " + amount + " bullets!");
    }
    protected virtual void GiveShirukenAmmo(int amount)
    {
        currentshirukenAmmo += amount;

        if (currentshirukenAmmo >= maxShirukenAmmo)
            currentshirukenAmmo = maxShirukenAmmo;

        Debug.Log(gameObject.name + " was given " + amount + " shiruken!");
    }
    //Left blank because although all actors can die, they can do so in different ways.
    protected virtual void OnDeath()
    {
        Debug.Log("OnDeath not implemented on " + gameObject.name);
    }
    protected virtual IEnumerator DeathCoroutine()
    {
        attackingState = 0;
        actorAnimator.SetTrigger("die");
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}