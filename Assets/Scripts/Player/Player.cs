using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Dashing and attacking")]
    public int dashCost;
    [Range(0, 100)]
    public float dashLength = 4;
    [Range(0, 100)]
    public float trailLength = 4;
    [Range(0, 100)]
    public float attackStepLength = 4;

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
    WeaponManager weaponM;
    Transform trail;
    Animator anim;
    public int facing = 1;

    void Start()
    {
        trail = transform.FindChild("Trail");
        controller = GetComponent<Controller2D>();
        creature = GetComponent<PlayerCreature>();
        stats = creature.stats;
        anim = GetComponent<Animator>();
        weaponM = transform.GetComponentInChildren<WeaponManager>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        //print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
    }

    void Update()
    {
        if(transform.position.y < -100)
        {
            transform.position = Vector2.one;
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Dashing            
        if (Input.GetButtonDown("Dash") && stats.curStamina >= dashCost)
        {
            anim.SetTrigger("Dashing");
        }

        // Attacking  
        if (Input.GetButtonDown("Fire1") && stats.curStamina >= weaponM.weapons[weaponM.currentWeapon].useStaminaCost)
        {
            anim.SetFloat("AttackId", (float)weaponM.weapons[weaponM.currentWeapon].attackType / 10f);
            anim.SetTrigger("Attack");
        }

        #region Animation, stunned, animationBusy, flipping sprites

        anim.SetFloat("Input", Mathf.Abs(input.x));
        anim.SetBool("Grounded", controller.collisions.below);
        
        if (creature.stats.stunned || creature.stats.animationBusy)
        {
            input = Vector2.zero;
        }

        #region Fliping sprite

        if (input.x != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            facing = input.x > 0 ? 1 : -1;
        }         

        FlipAllSprites();

        #endregion

        #endregion

        #region Jumping

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
        sprites.AddRange(transform.FindChild("Trail").GetComponentsInChildren<SpriteRenderer>());
        sprites.Add(transform.FindChild("Armor").GetComponent<SpriteRenderer>());

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].flipX = facing == -1;
        }
    }

    #region AnimationEvents + stamina costs

    void AnimationDash()
    {
        AnimationDrainStaminaDash();
        AnimationDashStep();
        AnimationInvincibility(0.1f);
    }

    void AnimationDrainStamina()
    {
        stats.curStamina -= weaponM.weapons[weaponM.currentWeapon].useStaminaCost;
        stats.DelayStaminaRegen();
    }

    void AnimationDrainStaminaDash()
    {
        stats.curStamina -= dashCost;
        stats.DelayStaminaRegen();
    }

    void AnimationAttackSlash(int hitCount)
    {
        if (hitCount < 0)
            return;

        if (weaponM.equippedWeapon.aoeObject.Length < hitCount)
            AnimationAttackSlash(hitCount - 1);

        if (weaponM.equippedWeapon.aoeObject[hitCount] != null)
        {
            GameObject swing = weaponM.equippedWeapon.aoeObject[hitCount];
            swing.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Swing");
            swing.transform.localScale = new Vector2(facing, 1);            
        }   
    }

    void AnimationInvincibility(float dur)
    {
        stats.Invincibility(dur);
    }

    void AnimationDashStep()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
            velocity.x = dashLength * input;
        else
            velocity.x = dashLength * facing;

        controller.Move(velocity * Time.deltaTime);
    }    

    void AnimationAttackStep()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
        {
            velocity.x = attackStepLength * input;
            facing = input < 0 ? -1 : 1;
        }
        else
            velocity.x = attackStepLength * facing;

        Vector2 hitboxPos = transform.FindChild("Hitbox").localPosition;

        if (transform.FindChild("Hitbox") != null)
        {
            if (facing > 0)
            {
                hitboxPos.x = Mathf.Abs(hitboxPos.x);
            }
            else if (facing < 0)
            {
                hitboxPos.x = -Mathf.Abs(hitboxPos.x);
            }

            transform.FindChild("Hitbox").localPosition = hitboxPos;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    #endregion
}