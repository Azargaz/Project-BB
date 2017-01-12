﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(EnemyCreature))]
public class EnemyAI : MonoBehaviour
{    
    protected enum EnemyState { idle, stop, walk, attack };
    protected EnemyState currentState = EnemyState.idle;

    [Header("Enemy AI")]
    public float range = 10f;    
    public float followRange = 1f;
    public float minDistanceFromTarget = 0.5f;

    public bool canJump = true;
    public bool canDropDown = true;
    protected bool jump = false;

    public float attackRange = 1f;
    public float attackCooldown = 1f;
    float attackTimer;    
    
    [HideInInspector]
    public bool attacked;

    [HideInInspector]
    public Vector3 velocity;
    public Vector3 storedVelocity;
    
    protected Vector2 playerDirection = Vector2.zero; 

    protected Animator anim;
    protected Controller2D controller;
    protected EnemyCreature creature;

    [Header("Pathfinding")]
    public bool pathfinding = true;
    protected Transform target;
    public float updateRate = 10f;
    Seeker seeker;
    public Path path;
    [HideInInspector]
    public bool pathIsEnded = false;
    public float nextWaypointDistance = 3;
    int currentWaypoint = 0;        

    [Header("")]
    float freezeSmoothTime = 0.05f;
    public bool freeze;
    public bool debugAI;
    public bool debugPathfinding;

    protected virtual void Start()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        attackTimer = attackCooldown;
        controller = GetComponent<Controller2D>();
        creature = GetComponent<EnemyCreature>();
        anim = GetComponent<Animator>();
        
        if(pathfinding && GetComponent<Seeker>() != null)
        {
            seeker = GetComponent<Seeker>();
            StartCoroutine(UpdatePath());
        }
    }

    protected virtual void Update()
    {
        if (freeze && creature.stats.alive)
        {
            anim.speed = Mathf.Lerp(anim.speed, 0, freezeSmoothTime);
            storedVelocity = velocity;
            velocity = Vector3.Lerp(storedVelocity, Vector3.zero, freezeSmoothTime);
            freezeSmoothTime += Time.deltaTime / 10000f;
            controller.Move(velocity * Time.deltaTime);
            return;
        }
        else
        {
            anim.speed = 1;
        }

        if (!creature.stats.alive)
            return;

        if (!canJump)
            anim.SetBool("Grounded", true);
        else
            anim.SetBool("Grounded", controller.collisions.below);

        currentState = pathfinding ? SearchTargetWithPathfinding() : SearchTarget();

        // Attack & SearchPlayer cooldown
        attackTimer -= Time.deltaTime;
    }

    #region AI without Pathfinding

    protected EnemyState SearchTarget()
    {
        // If no player return
        if (target == null)
        {
            if (debugAI)
                Debug.LogError("GroundEnemyAI can't find player.");
            return EnemyState.idle;
        }

        Vector2 targetPosDifference = target.position - transform.position;
        float distToTarget = Vector2.Distance(target.position, transform.position);
        controller.jumpDown = false;

        // If player is out of range return
        if (distToTarget > range)
        {
            if (debugAI)
                Debug.Log("Player out of range.");

            return EnemyState.idle;
        }

        if((controller.collisions.right && targetPosDifference.x > 0) || (controller.collisions.left && targetPosDifference.x < 0))
        {
            return EnemyState.stop;
        }

        // If player is in attack range, attack - if it's on cooldown, stop
        if (attackTimer <= 0 && !attacked)
        {
            if (distToTarget <= Random.Range(minDistanceFromTarget, attackRange))
            {
                playerDirection = new Vector2(targetPosDifference.x > 0 ? 1 : -1, 0);
                attacked = true;
                return EnemyState.attack;
            }
            else
            {             
                if (Mathf.Abs(targetPosDifference.x) > minDistanceFromTarget)
                {
                    playerDirection = new Vector2(targetPosDifference.x > 0 ? 1 : -1, Mathf.Abs(targetPosDifference.y) < 1f ? 0 : targetPosDifference.y > 0 ? (canJump ? 1 : 0) : (canDropDown ? -1 : 0));

                    controller.jumpDown = playerDirection.y == -1;

                    return EnemyState.walk;
                }

                return EnemyState.stop;
            }
        }
        // Else walk towards player
        else
        {
            if(distToTarget > followRange && Mathf.Abs(targetPosDifference.x) > minDistanceFromTarget)
            {
                playerDirection = new Vector2(targetPosDifference.x > 0 ? 1 : -1, Mathf.Abs(targetPosDifference.y) < 1f ? 0 : targetPosDifference.y > 0 ? (canJump ? 1 : 0) : (canDropDown ? -1 : 0));

                controller.jumpDown = playerDirection.y == -1;

                return EnemyState.walk;
            }            
            // If player is too close to enemy, stop          
            else
            {
                return EnemyState.stop;
            }
        }
    }
    
    #endregion 

    #region Pathfinding

    EnemyState SearchTargetWithPathfinding()
    {
        // If no player return
        if (target == null)
        {
            if (debugAI)
                Debug.LogError("GroundEnemyAI can't find player.");
            return EnemyState.idle;
        }

        Vector2 targetPosDifference = target.position - transform.position;
        float distToTarget = Vector2.Distance(target.position, transform.position);

        // If player is out of range return
        if (distToTarget > range)
        {
            if (debugAI)
                Debug.Log("Player out of range.");

            return EnemyState.idle;
        }

        // If player is in attack range and attack isn't on cooldown, attack
        if (attackTimer <= 0 && !attacked && controller.collisions.below)
        {
            if (distToTarget <= Random.Range(minDistanceFromTarget, attackRange))
            {
                playerDirection = new Vector2(targetPosDifference.x > 0 ? 1 : -1, 0);
                attacked = true;
                return EnemyState.attack;
            }
        }

        // Else walk towards player        
        if (distToTarget > minDistanceFromTarget)
        {
            if ((!canJump || !canDropDown) && Mathf.Abs(targetPosDifference.x) < minDistanceFromTarget)
                return EnemyState.stop;

            return EnemyState.walk;
        }
        // If player is too close to enemy, stop          
        else
        {
            return EnemyState.stop;
        }
        
    }

    IEnumerator UpdatePath()
    {
        if (target == null || !creature.stats.alive)
        {
            yield return null;
        }

        float distToTarget = Vector2.Distance(target.transform.position, transform.position);

        if (pathfinding && !freeze && distToTarget < range)
            seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1 / updateRate);

        StartCoroutine(UpdatePath());
    }

    protected void OnPathComplete(Path p)
    {
        Debug.Log("Path ready. Any errors go here >>> " + (p.error ? "panic mode: on" : "trash"));
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    protected void Pathfinding()
    {
        float distToTarget = Vector2.Distance(target.transform.position, transform.position);

        if (distToTarget < minDistanceFromTarget)
            path = null;

        Vector2 normalizedDirection = Vector2.zero;
        Vector2 input = Vector2.zero;        
        float dist = 0;

        if (path != null && target != null)
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {
                Debug.Log("End of path reached.");
                pathIsEnded = true;
            }
            else
            {
                #region Pathfinding

                pathIsEnded = false;
                normalizedDirection = (path.vectorPath[currentWaypoint] - transform.position).normalized;               
                input = new Vector2(Mathf.Abs(normalizedDirection.x) == 0 ? 0 : normalizedDirection.x > 0 ? 1 : -1, Mathf.Abs(normalizedDirection.y) == 0 ? 0 : normalizedDirection.y > 0 ? 1 : -1);

                if (debugPathfinding)
                {
                    Debug.Log("Input: " + input);
                }

                dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

                #endregion

                #region Jumping/Jumping down platforms

                jump = false;

                if (controller.collisions.below)
                    controller.jumpDown = false;

                if(canDropDown)
                {
                    if(!controller.collisions.below && input.y < 0 && controller.jumpDown)
                        seeker.StartPath(transform.position, target.position, OnPathComplete);
                }

                if (canJump)
                {
                    if (input.y == 1 && Mathf.Abs(normalizedDirection.x) < 0.2f)
                        jump = true;

                    if (controller.collisions.left || controller.collisions.right)
                        jump = true;

                    // Reset jump
                    if (normalizedDirection.y <= 0)
                        jump = false;
                }

                // Droping down on platforms
                if (canDropDown)
                {
                    if (input.y < 0 && controller.collisions.below)
                    {
                        controller.jumpDown = true;
                    }
                    else if (input.y >= 0)
                        controller.jumpDown = false;
                }

                #endregion
            }
        }
        else if(path == null)
        {
            playerDirection.x = 0;
        }

        playerDirection.y = input.y;

        if (input.x != 0)
            playerDirection.x = input.x;   

        if (path == null || currentWaypoint >= path.vectorPath.Count || pathIsEnded || dist == 0)
            return;

        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }

    #endregion

    void AnimationAttackOnCD()
    {
        attackTimer = attackCooldown;
        attacked = false;
    }
}
