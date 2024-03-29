﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAI : EnemyAI
{
    [Header("Ground AI")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    protected float accelerationTimeAirborne = .2f;
    protected float accelerationTimeGrounded = .1f;
    public float movementSpeed;
    protected float moveSpeed = 6;

    protected float gravity;
    protected float maxJumpVelocity;
    protected float minJumpVelocity;
    protected float velocityXSmoothing;
    protected int facing = 1;

    [Header("Attacks")]
    public Attack[] attacks;

    [System.Serializable]
    public class Attack
    {
        public string name;
        public int attackID;
        public int chanceToSpawnWeight;
    }

    [Header("")]
    public bool debug;

    protected override void Start ()
    {
        base.Start();
        
        moveSpeed = Random.Range(movementSpeed - 0.1f, movementSpeed + 0.1f);

        RecalculateJumpVelocity();
    }

    protected void RecalculateJumpVelocity()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    protected override void Update ()
    {
        base.Update();

        if (creature.stats.pause)
            return;

        if (freeze)
            return;

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
                
        Vector2 input = targetDirection;        

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
                    if(pathfinding)
                        Pathfinding();
                    
                    input = targetDirection;

                    if(runaway)
                    {
                        runaway = false;
                        input.x *= -1;
                    }

                    break;
                }
            case EnemyState.attack:
                {
                    creature.stats.animationBusy = true;
                    input.y = 0;
                    Animations(input.x);
                    anim.SetTrigger("Attack");             
                    break;
                }
        }

        #endregion     

        // Block movement if stunned or busy
        if (creature.stats.animationBusy)
        {
            input = Vector2.zero;
        }

        if(creature.stats.stunned)
        {
            input = Vector2.zero;
        }

        if ((controller.collisions.right && input.x == 1) || (controller.collisions.left && input.x == -1))            
        {
            input.x = 0;
        }

        Animations(input.x);

        #region Jumping        

        if (input.y < 0)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }
        if (input.y > 0 && controller.collisions.below)
        {
            if (controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
            }
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
            facing = input > 0 ? 1 : -1;
            
            if (transform.Find("Hitbox") != null)
            {
                Vector2 hitboxPos = transform.Find("Hitbox").localPosition;
                hitboxPos.x = facing * Mathf.Abs(hitboxPos.x);
                transform.Find("Hitbox").localPosition = hitboxPos;
            }

            if(transform.Find("Projectile") != null)
            {
                transform.Find("Projectile").localScale = new Vector3(facing, 1, 1);
            }

            GetComponent<SpriteRenderer>().flipX = facing < 0;            
        }

        anim.SetFloat("Input", Mathf.Abs(input));
        //anim.SetBool("Grounded", controller.collisions.below);
    }

    void AnimationSpawnProjectile()
    {
        if(GetComponentInChildren<SpawnProjectile>() != null)
        {
            GetComponentInChildren<SpawnProjectile>().AnimationSpawnProjectile();
        }
    }

    protected int RollWithWeights(Attack[] array)
    {
        int summedWeights = 0;
        int returnInt = 0;

        if (array.Length <= 1)
            return 0;

        for (int x = 0; x < array.Length; x++)
        {
            summedWeights += array[x].chanceToSpawnWeight;
        }

        for (int x = 0; x < array.Length; x++)
        {
            int random = Random.Range(0, summedWeights);
            random -= array[x].chanceToSpawnWeight;

            if (random <= 0)
            {
                returnInt = array[x].attackID;
                break;
            }
        }

        return returnInt;
    }
}
