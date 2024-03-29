﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringGhostEnemyAI : EnemyAI
{
    public float movementSpeed;
    float moveSpeed = 6;
    float accelerationTime = .1f;
    float circleTimer = 0;

    Vector2 velocitySmoothing;
    int facing = 1;

    [Header("")]
    public bool debug;

    protected override void Start()
    {
        base.Start();

        moveSpeed = Random.Range(movementSpeed - 0.5f, movementSpeed + 0.5f);
    }

    protected override void Update()
    {
        base.Update();

        if (creature.stats.pause)
            return;

        if (freeze)
            return;

        Vector2 input = Vector2.zero;

        switch (currentState)
        {
            case EnemyState.idle:
                {
                    input = Vector2.zero;
                    break;
                }
            case EnemyState.stop:
                {
                    circleTimer += Time.deltaTime;
                    input = Circle(target.position);
                    break;
                }
            case EnemyState.walk:
                {                    
                    input = targetDirection;
                    break;
                }
            case EnemyState.attack:
                {
                    creature.stats.animationBusy = true;
                    anim.SetTrigger("Attack");
                    input = Vector2.zero;
                    break;
                }
        }

        // Block movement if stunned or busy
        if (creature.stats.stunned || creature.stats.animationBusy)
            input = Vector2.zero;

        Animations(input.x != 0 ? input.x : input.y);

        #region Movement stuff (gravity, moving)

        Vector2 targetVelocity = input * moveSpeed;
        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref velocitySmoothing, accelerationTime, Mathf.Infinity, Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        #endregion
    }

    Vector2 Circle(Vector2 S)
    {
        Vector2 direction;

        float R = attackRange + 3;

        float angle = circleTimer * 2f;

        float X = S.x + Mathf.Sin(angle) * R;
        float Y = S.y + Mathf.Cos(angle) * R;

        direction = (new Vector3(X, Y) - transform.position).normalized;

        return direction;
    }

    void Animations(float input)
    {
        if (input != 0)
        {
            facing = (int)targetDirection.x;
            Vector2 hitboxPos = transform.Find("Hitbox").localPosition;

            if (transform.Find("Hitbox") != null)
            {
                if (facing > 0)
                {
                    hitboxPos.x = Mathf.Abs(hitboxPos.x);
                }
                else if (facing < 0)
                {
                    hitboxPos.x = -Mathf.Abs(hitboxPos.x);
                }

                transform.Find("Hitbox").localPosition = hitboxPos;
            }

        }

        GetComponent<SpriteRenderer>().flipX = facing < 0;

        anim.SetFloat("Input", Mathf.Abs(input));
    }
}
