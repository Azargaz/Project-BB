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
    public float moveSpeed = 6;

    bool jump;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    LivingCreature creature;
    Animator anim;
    int facing = 1;

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

        Vector2 input;
        input.x = SearchPlayer();

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input.x = 0;

        attackTimer -= Time.deltaTime;

        #region Animations

        if (input.x != 0)
        {
            facing = (int)input.x;
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

        anim.SetFloat("Input", Mathf.Abs(input.x));
        //anim.SetBool("Grounded", controller.collisions.below);

        // Attack
        if (Mathf.Abs(input.x) == 2)
        {
            creature.stats.animationBusy = true;
            anim.SetTrigger("Attack");
            return;
        }

        #endregion

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

    int SearchPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player == null)
        {
            Debug.LogError("GroundEnemyAI can't find player.");
            return 0;
        }

        float distanceToPlayer = player.transform.position.x - transform.position.x;

        if (Mathf.Abs(distanceToPlayer) > range)
        {
            Debug.Log("Player out of range.");
            return 0;
        }

        int playerDirection;

        if (distanceToPlayer == 0)
        {
            playerDirection = 0;
        }
        else if (distanceToPlayer < attackRange && distanceToPlayer > -attackRange)
        {
            if(attackTimer <= 0)
            {
                playerDirection = distanceToPlayer > 0 ? 2 : -2;
                attackTimer = attackCooldown;
            }
            else
            {
                playerDirection = 0;
            }
        }
        else
        {
            if (distanceToPlayer > 0)
                playerDirection = 1;
            else
                playerDirection = -1;
        }

        return playerDirection;
    }
}
