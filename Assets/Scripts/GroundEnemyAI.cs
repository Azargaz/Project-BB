using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAI : MonoBehaviour
{
    public float range;
    public float attackRange;
    public float attackCooldown;
    float attackTimer;

    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float movementSpeed;
    float moveSpeed = 6;

    bool jump = false;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    LivingCreature creature;
    Animator anim;

    enum EnemyState { idle, stop, walk, attack };
    EnemyState currentState = EnemyState.idle;
    int playerDirection = 0;
    int facing = 1;
    int idleMovement = 0;

    void Start ()
    {
        controller = GetComponent<Controller2D>();
        creature = GetComponent<LivingCreature>();
        anim = GetComponent<Animator>();
        attackTimer = attackCooldown;

        #region Jumping

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        #endregion
    }

    void Update ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if (currentState == EnemyState.idle)
            moveSpeed = movementSpeed / 3;
        else
            moveSpeed = movementSpeed;

        Vector2 input;
        currentState = SearchPlayer();
        input.x = playerDirection;

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

        // Attack cooldown
        attackTimer -= Time.deltaTime;

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input.x = 0;

        Animations(input.x);

        #region Jumping

        if (jump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
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
        if(Mathf.Abs(idleMovement) == 0)
        {
            idleMovement = 300;
            facing *= -1;
            return 0;
        }
        else
        {
            idleMovement--;
            return 1;
        }
    }

    EnemyState SearchPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // If no player return
        if(player == null)
        {
            Debug.LogError("GroundEnemyAI can't find player.");
            return EnemyState.idle;
        }

        float distanceToPlayerX = player.transform.position.x - transform.position.x;
        float distanceToPlayerY = player.transform.position.y - transform.position.y;

        // If player is out of range return
        if (Mathf.Abs(distanceToPlayerX) > range)
        {
            Debug.Log("Player out of range.");
            return EnemyState.idle;
        }

        // If player is on top of enemy, stop
        if (distanceToPlayerX == 0)
        {
            return EnemyState.stop;
        }
        // If player is in attack range, attack - if it's on cooldown, stop
        else if (distanceToPlayerX < attackRange && distanceToPlayerX > -attackRange)
        {
            if(attackTimer <= 0 && Mathf.Abs(distanceToPlayerY) < attackRange)
            {
                playerDirection = distanceToPlayerX > 0 ? 1 : -1;
                attackTimer = attackCooldown;
                return EnemyState.attack;
            }
            else
            {
                return EnemyState.stop;
            }
        }
        // Else walk towards player
        else
        {
            if (distanceToPlayerX > 0)                
            {
                playerDirection = 1;
                return EnemyState.walk;
            }
            else
            {
                playerDirection = -1;
                return EnemyState.walk;
            }
        }
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
