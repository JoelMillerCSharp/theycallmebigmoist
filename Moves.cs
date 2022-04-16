using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles the movement and collision for all actors capable of movement.
public class Moves : Fights
{
    //Moving Actor Movement Variables
    protected Vector3 moveVector;
    [SerializeField]
    protected float moveSpeed;
    protected float jumpHeight = .5f;
    [SerializeField]
    protected Transform jumpChecker;
    [SerializeField]
    protected LayerMask Ground;
    protected float groundDistance = .2f;
    protected bool isGrounded = true;
    protected bool canDoubleJump = false;
    protected Rigidbody rBody;
    protected Vector3 originalFacing;
    protected bool lastMovedRight;
    protected bool canMove = true;
    
    //Advanced movement Variables
    protected bool grabbedLedge = false;
    protected bool canDash = false;
    protected bool canRoll = true;
    protected float dashTimer = .25f;
    public float dashCooldownTimer;
    public float rollCooldownTimer = 2f;

    protected override void Awake()
    {
        //Get necessary components for the average moving actor.
        rBody = GetComponent<Rigidbody>();
    }
    protected override void Start()
    {
        //Initializing base parameters for the average moving actor.
        base.Start();


        canMove = true;
        isAlive = true;
        canDoubleJump = false;
        grabbedLedge = false;
        canDash = false;
        canRoll = true;
        lastMovedRight = true;
        jumpHeight = 3;
        dashTimer = 0;
        dashCooldownTimer = 0;
        originalFacing = transform.localScale;
    }
    protected virtual IEnumerator Dash()
    {
        //Zero out velocity, add some force to the actor based on their facing direction, then put the dash ability on cooldown.
        rBody.useGravity = false;
        rBody.velocity = Vector3.zero;
        canMove = false;

        rBody.AddForce(Mathf.Sqrt(600) * Vector3.right * transform.localScale.x, ForceMode.VelocityChange);
        dashCooldownTimer = 0;
        actorAnimator.SetTrigger("dash");

        yield return new WaitForSeconds(.125f);

        rBody.useGravity = true;
        canMove = true;
        rBody.velocity = Vector3.zero;
    }
    protected virtual IEnumerator Roll()
    {
        rBody.useGravity = false;
        rBody.velocity = Vector3.zero;
        canMove = false;
        invincible = true;
        canRoll = false;
        rollCooldownTimer = 0;
        actorAnimator.SetTrigger("roll");

        //Put in Animation functionality for the Roll here.
        rBody.AddForce(Mathf.Sqrt(300) * Vector3.right * transform.localScale.x, ForceMode.VelocityChange);

        yield return new WaitForSeconds(.25f);

        rBody.useGravity = true;
        canMove = true;
        rBody.velocity = Vector3.zero;
        invincible = false;
    }
    public void ClimbLedge()
    {
        //Do the same thing we did for double jumping here.
        if (grabbedLedge)
            return;
        rBody.velocity = Vector3.zero;
        rBody.AddForce(Vector3.up * Mathf.Sqrt(-3f * Physics.gravity.y), ForceMode.VelocityChange);
        grabbedLedge = true;
        actorAnimator.SetTrigger("climb");
    }
    protected virtual void Jump()
    {
        //Zero out the velocity to avoid losing height gain when double jumping.
        rBody.velocity = Vector3.zero;
        rBody.AddForce(Vector3.up * Mathf.Sqrt(-1.5f * jumpHeight * Physics.gravity.y), ForceMode.VelocityChange);
        grabbedLedge = false;
        audioHandler.PunchSound();

        if (isGrounded)
            return;
        canDoubleJump = false;
        actorAnimator.SetTrigger("djump");
    }
    protected override IEnumerator Uppercut()
    {
        attackingState = 5;
        actorAnimator.SetTrigger("uppercut");
        audioHandler.PunchSound();

        yield return new WaitForSeconds(.25f);

        Jump();

        yield return new WaitForSeconds(.5f);
        attackingState = 0;
    }
    protected virtual void FixedUpdate()
    {
        //Checks if the actor can jump, dash, roll and grab ledges.
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            canDash = true;
        }
        else if (dashTimer <= 0)
            canDash = false;

        if (dashCooldownTimer < 5)
        {
            canDash = false;
            dashCooldownTimer += Time.deltaTime;
        }
        if (rollCooldownTimer > 2 && isGrounded)
            canRoll = true;
        else
        {
            canRoll = false;
            rollCooldownTimer += Time.deltaTime;
        }

        //Checks if the actor is currently on the ground.
        isGrounded = Physics.CheckSphere(jumpChecker.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);
        if (isGrounded)
        {
            if (attackingState == 6)
                attackingState = 0;
            canDoubleJump = true;
            grabbedLedge = false;
            actorAnimator.SetBool("onGround", true);
        }
        else
            actorAnimator.SetBool("onGround", false);
    }
    protected virtual void MoveActor(Vector3 input)
    {
        if (!isAlive)
            return;
        //Handles both player input and enemy movement parameters.
        moveVector = new Vector3(input.x * moveSpeed, 0, 0);

        //Apply knockback (and recover from it) if there is any.
        moveVector += knockbackDirection;
        knockbackDirection = Vector3.Lerp(knockbackDirection, Vector3.zero, knockbackRecoverySpeed);

        //Move the actor, rotating them in the right direction. Also applies the walking animation.
        if (canMove)
            transform.Translate(moveVector.x * Time.deltaTime, 0, 0);
        if (moveVector.x > 0)
        {
            facingRight = true;
            transform.localScale = originalFacing;
            if (isGrounded)
                actorAnimator.SetBool("isMoving", true);
        }
        else if (moveVector.x < 0)
        {
            facingRight = false;
            transform.localScale = new Vector3(originalFacing.x * -1, originalFacing.y, originalFacing.z);
            if (isGrounded)
                actorAnimator.SetBool("isMoving", true);
        }
        else
            actorAnimator.SetBool("isMoving", false);
    }
}