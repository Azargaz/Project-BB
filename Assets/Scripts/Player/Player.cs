using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Dashing and attacking")]
    public int dashCost;
    public int[] attackCosts = new int[0];
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
    Transform trail;
    Animator anim;
    public int facing = 1;
    public GameObject attackSlash;

    void Start()
    {
        trail = transform.FindChild("Trail");
        controller = GetComponent<Controller2D>();
        creature = GetComponent<PlayerCreature>();
        stats = creature.stats;
        anim = GetComponent<Animator>();

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
        if (Input.GetButtonDown("Fire1") && stats.curStamina >= attackCosts[0])
        {
            anim.SetTrigger("Attack");
        }

        #region Animation

        anim.SetFloat("Input", Mathf.Abs(input.x));
        anim.SetBool("Grounded", controller.collisions.below);
        
        if (creature.stats.stunned)
        {
            input = Vector2.zero;
        }

        #region Fliping sprite and hitbox

        if (input.x != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            facing = input.x > 0 ? 1 : -1;
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
        }            

        GetComponent<SpriteRenderer>().flipX = facing == -1;
        FlipAllChildrenSprites();

        #endregion

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
            input = Vector2.zero;

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

    void FlipAllChildrenSprites()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].flipX = facing == -1;
        }
    }

    #region AnimationEvents + stamina costs

    public enum StaminaCosts { dash, attack1 };

    void AnimationDrainStamina(StaminaCosts cost)
    {
        switch (cost)
        {
            case StaminaCosts.dash:
                {
                    stats.curStamina -= dashCost;
                    break;
                }
            case StaminaCosts.attack1:
                {
                    stats.curStamina -= attackCosts[0];
                    break;
                }
        }

        stats.DelayStaminaRegen();
    }

    void AnimationAttackSlash()
    {
        if (attackSlash != null)
        {
            GameObject clone = Instantiate(attackSlash, transform);
            clone.transform.position = new Vector2(transform.position.x + 0.5f * facing, transform.position.y);
            FlipAllChildrenSprites();
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