using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAI : EnemyAI
{
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float movementSpeed;
    float moveSpeed = 6;

    public bool canJump;
    bool jump = false;

    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    int facing = 1;

    Controller2D controller;
    EnemyCreature creature;
    Animator anim;

    [Header("")]
    public bool debug;

    protected override void Start ()
    {
        base.Start();

        controller = GetComponent<Controller2D>();
        creature = GetComponent<EnemyCreature>();
        anim = GetComponent<Animator>();
        moveSpeed = Random.Range(movementSpeed - 0.5f, movementSpeed + 0.5f);
        
        if(creature.enemySize > 1)
        {
            attackRange += (attackRange * (creature.enemySize - 1)) / 2;
        }

        #region Jumping

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        #endregion
    }

    protected override void Update ()
    {
        base.Update();

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input;
        input.x = playerDirection.x;

        switch(currentState)
        {
            case EnemyState.idle:
                {
                    input.x = Idle() * facing;
                    break;
                }
            case EnemyState.stop:
                {
                    input.x = 0;
                    break;
                }
            case EnemyState.walk:
                {
                    break;
                }
            case EnemyState.attack:
                {                    
                    creature.stats.animationBusy = true;
                    anim.SetTrigger("Attack");
                    Animations(input.x);
                    input.x = 0;
                    break;
                }
        }

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input.x = 0;

        if((input.x > 0 && controller.collisions.right) || (input.x < 0 && controller.collisions.left) && canJump)
        {
            jump = true;
        }

        Animations(input.x);

        #region Jumping

        if (jump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
            jump = false;
        }
        else if(!controller.collisions.below)
        {
            jump = false;
        }

        #endregion

        #region Movement stuff (gravity, moving)

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        #endregion
    }

    int Idle()
    {
        return 0;
    }

    void Animations(float input)
    {
        if (input != 0)
        {
            facing = (int)input;
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

        GetComponent<SpriteRenderer>().flipX = facing < 0;

        anim.SetFloat("Input", Mathf.Abs(input));
        //anim.SetBool("Grounded", controller.collisions.below);
    }
}
