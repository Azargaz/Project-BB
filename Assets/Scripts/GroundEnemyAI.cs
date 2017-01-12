using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class GroundEnemyAI : EnemyAI
{
    [Header("GroundEnemy AI")]
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float movementSpeed;
    float moveSpeed = 6;

    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    int facing = 1;

    [Header("")]
    public bool debug;

    protected override void Start ()
    {
        pathfinding = true;

        base.Start();
        
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

        if (freeze)
            return;

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = playerDirection;
        Pathfinding();

        #region EnemyStates

        switch (currentState)
        {
            case EnemyState.idle:
                {
                    input = Vector2.zero;
                    break;
                }
            case EnemyState.stop:
                {
                    input.x = 0;

                    if (jump)
                        jump = false;

                    break;
                }
            case EnemyState.walk:
                {                    
                    break;
                }
            case EnemyState.attack:
                {
                    input.x = 0;
                    creature.stats.animationBusy = true;
                    anim.SetTrigger("Attack");
                    Animations(input.x);                    
                    break;
                }
        }

        #endregion

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input = Vector2.zero;

        if ((controller.collisions.right && input.x == 1) || (controller.collisions.left && input.x == -1))
            input.x = 0;

        Animations(input.x);        

        #region Jumping

        if (jump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;           
            jump = false;
        }
        else if (!controller.collisions.below)
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
