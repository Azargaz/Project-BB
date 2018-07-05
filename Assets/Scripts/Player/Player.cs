using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Dashing")]
    public int dashCost;
    [Range(0, 100)]
    public float dashLength = 4;
    [Range(0, 100)]
    public float trailLength = 4;

    [Header("Movement")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerCreature creature;
    LivingCreature.Statistics stats;
    Transform trail;
    Animator anim;
    public int facing = 1;
    public bool canDash = true;
    public bool freeze;
    public bool mouseChangingDirections = false;
    Vector2 mousePos;
    int mouseSide;
    bool changeAttackDirection = false;

    void Start()
    {
        trail = transform.Find("Trail");
        controller = GetComponent<Controller2D>();
        creature = GetComponent<PlayerCreature>();
        stats = creature.stats;
        anim = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            mouseChangingDirections = !mouseChangingDirections;

        if (stats.pause)
            return;

        if (freeze)
            return;

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
            controller.jumpDown = false;
        }

        #region Input, mousePos

        Vector2 input = Vector2.zero;

        if (stats.alive)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseSide = (mousePos.x - transform.position.x) >= 0 ? 1 : -1;

            if (InputControl.UsingGamepad())
                mouseSide = Input.GetAxisRaw("XC Right Stick X") == 0 ? 0 : Input.GetAxisRaw("XC Right Stick X") > 0 ? 1 : -1;

            if (!mouseChangingDirections)
                mouseSide = 0;
        }

        #endregion

        #region Dashing

        if(stats.alive && canDash)
        {
            if (Input.GetButtonDown("Dash") && stats.curStamina >= dashCost)
            {
                anim.SetTrigger("Dashing");
            }
        }

        #endregion

        #region Animation, changing direction on attacks, stunned, animationBusy, flipping sprites

        anim.SetFloat("Input", Mathf.Abs(input.x));
        anim.SetBool("Grounded", controller.collisions.below);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            changeAttackDirection = true;
        }
        else 
        {
            if (changeAttackDirection)
            {
                changeAttackDirection = false;

                if (mouseSide != 0)
                    facing = mouseSide;
            }                
        }

        #region Fliping sprite

        if (input.x != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            facing = input.x > 0 ? 1 : -1;
        }

        if (input.x == 0 && mouseSide != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {            
            facing = mouseSide;
        }

        FlipAllSprites();

        #endregion

        if (stats.stunned || stats.animationBusy)
        {
            input = Vector2.zero;
        }

        #endregion

        #region Jumping/Jumping down platforms

        if (input.y < 0)
            controller.jumpDown = true;

        if(stats.alive)
        {
            if (Input.GetButtonDown("Jump") && controller.collisions.below && !anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
            {
                if (controller.collisions.below)
                {
                    velocity.y = maxJumpVelocity;
                }
            }
            if (Input.GetButtonUp("Jump"))
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }
        }

        #endregion

        #region Movement stuff + dash trail

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        #region Dash trail

        for (int i = 0; i < trail.childCount; i++)
        {
            trail.GetChild(i).localPosition = -velocity * i / (100 - trailLength + 1);
        }

        #endregion

        // Dash without gravity below VVVVV

        //if (!anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
        //    velocity.y += gravity * Time.deltaTime;
        //else
        //    velocity.y = 0;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        #endregion
    }

    void FlipAllSprites()
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        sprites.Add(GetComponent<SpriteRenderer>());
        sprites.AddRange(transform.Find("Trail").GetComponentsInChildren<SpriteRenderer>());
        sprites.Add(transform.Find("Armor").GetComponent<SpriteRenderer>());

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].flipX = facing == -1;
        }
    }

    public void Dash(float distance, int dashDirection, bool changeDirection)
    {
        velocity.x = distance * facing * dashDirection;

        controller.Move(velocity * Time.deltaTime);

        if (changeDirection)
            facing = -facing;
    }

    #region AnimationEvents

    void AnimationDash()
    {
        stats.curStamina -= dashCost;
        stats.DelayStaminaRegen();

        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
            velocity.x = dashLength * input;
        else
            velocity.x = dashLength * facing;

        controller.Move(velocity * Time.deltaTime);

        creature.AnimationInvincibility(0.1f);
    }

    #endregion
}