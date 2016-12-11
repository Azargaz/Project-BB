using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Range(0, 100)]
    public float dashLength = 4;
    [Range(0, 100)]
    public float trailLength = 4;
    [Range(0, 100)]
    public float attackStepLength = 4;
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerCreature creature;
    Transform trail;
    Animator anim;
    int facing = 1;

    void Start()
    {
        trail = transform.FindChild("Trail");
        controller = GetComponent<Controller2D>();
        creature = GetComponent<PlayerCreature>();
        anim = GetComponent<Animator>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        
        //print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        #region Animation

        anim.SetFloat("Input", Mathf.Abs(input.x));
        anim.SetBool("Grounded", controller.collisions.below);
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player") || creature.stats.stunned)
            input = Vector2.zero;

        #region Fliping sprites and hitbox

        if (input.x != 0)
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
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].flipX = facing == -1;
        }

        #endregion

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
            input = Vector2.zero;

        #endregion

        #region Jumping

        //if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below && !anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
        //{
        //    velocity.y = jumpVelocity;
        //}

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

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        #endregion
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

    void AnimationDashStep()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
            velocity.x = dashLength * input;
        else
            velocity.x = dashLength * facing;

        controller.Move(velocity * Time.deltaTime);
    }
}