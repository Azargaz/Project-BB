using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAI : MonoBehaviour
{
    public float searchPlayerInterval;
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
    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    EnemyCreature creature;
    Animator anim;
    GameObject player;

    enum EnemyState { idle, stop, walk, attack };
    EnemyState currentState = EnemyState.idle;
    int playerDirection = 0;
    int facing = 1;
    int idleMovement = 0;
    float searchPlayerDelay = 0;
    Vector2 playerPos;

    [Header("")]
    public bool debug;

    void Start ()
    {
        controller = GetComponent<Controller2D>();
        creature = GetComponent<EnemyCreature>();
        anim = GetComponent<Animator>();
        attackTimer = attackCooldown;
        moveSpeed = Random.Range(movementSpeed - 0.5f, movementSpeed + 0.5f);
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform.position;
        
        if(creature.enemySize > 1)
        {
            attackRange += (attackRange * (creature.enemySize - 1)) / 2;
        }

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

        // Attack & SearchPlayer cooldown
        attackTimer -= Time.deltaTime;
        searchPlayerDelay -= Time.deltaTime;

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input.x = 0;

        if((input.x > 0 && controller.collisions.right) || (input.x < 0 && controller.collisions.left))
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
        return -0;
    }

    EnemyState SearchPlayer()
    {        
        // If no player return
        if(player == null)
        {
            if(debug)
                Debug.LogError("GroundEnemyAI can't find player.");
            return EnemyState.idle;
        }

        if (searchPlayerDelay <= 0)
        {
            playerPos = player.transform.position;
            searchPlayerDelay = searchPlayerInterval;
        }        

        Vector2 distanceToPlayer = (Vector3) playerPos - transform.position;

        // If player is out of range return
        if (Mathf.Abs(distanceToPlayer.x) > range || Mathf.Abs(distanceToPlayer.y) > range)
        {
            if (debug)
                Debug.Log("Player out of range.");
            return EnemyState.idle;
        }

        // If player is on top of enemy, stop
        if (Mathf.Abs(distanceToPlayer.x) < 0.5f || (Mathf.Abs(distanceToPlayer.x) < 0.5f && Mathf.Abs(distanceToPlayer.y) < 0.5f))
        {
            return EnemyState.stop;
        }
        // If player is in attack range, attack - if it's on cooldown, stop
        else if (Mathf.Abs(distanceToPlayer.x) < attackRange && Mathf.Abs(distanceToPlayer.y) < attackRange)
        {
            if(attackTimer <= 0)
            {
                playerDirection = distanceToPlayer.x > 0 ? 1 : -1;
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
            playerDirection = distanceToPlayer.x > 0 ? 1 : -1;
            return EnemyState.walk;
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
