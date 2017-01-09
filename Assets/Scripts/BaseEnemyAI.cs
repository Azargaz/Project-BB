using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Seeker))]
public class BaseEnemyAI : MonoBehaviour
{
    public Transform target;

    public float updateRate = 2f;

    Seeker seeker;
    Controller2D controller;

    public Path path;

    public float movementSpeed = 6f;
    public bool canJump = true;
    public bool canDropDown = true;
    public float minDistanceFromTarget = 1;

    [HideInInspector]
    public bool pathIsEnded = false;

    public float nextWaypointDistance = 3;
    int currentWaypoint = 0;

    #region 2D Controller stuff
    Vector2 velocity;
    float gravity;
    float jumpVelocity;
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float velocityXSmoothing;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    #endregion

    void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("Can't find player.");
            return;
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            yield return null;
        }

        float distToTarget = Vector2.Distance(target.transform.position, transform.position);

        if(distToTarget > minDistanceFromTarget && controller.collisions.below)
            seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1 / updateRate);

        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("Path ready. Any errors go here >>> " + (p.error ? "panic mode: on" : "trash"));
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    bool jump;

    void Update()
    {
        float distToTarget = Vector2.Distance(target.transform.position, transform.position);

        if (distToTarget < minDistanceFromTarget)
            path = null;

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

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
                Vector2 normalizedDirection = (path.vectorPath[currentWaypoint] - transform.position).normalized;
                input = new Vector2(Mathf.Abs(normalizedDirection.x) == 0 ? 0 : normalizedDirection.x > 0 ? 1 : -1, Mathf.Abs(normalizedDirection.y) < 0.5f ? 0 : normalizedDirection.y > 0 ? 1 : -1);
                dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

                #endregion

                #region Jumping

                jump = false;
                controller.jumpDown = false;

                if(canJump)
                {
                    if (input.y == 1 && Mathf.Abs(normalizedDirection.x) < 0.2f)
                        jump = true;

                    if (controller.collisions.left || controller.collisions.right)
                        jump = true;

                    // Reset jump
                    if (normalizedDirection.y <= 0 || dist + 0.5f > jumpHeight)
                        jump = false;

                    // Jumping
                    if (jump && controller.collisions.below)
                    {
                        float distY = normalizedDirection.y * jumpHeight;

                        if (distY < jumpHeight / 2)
                        {
                            float grav = -(2 * (distY)) / Mathf.Pow(timeToJumpApex, 2);
                            float jumpVel = Mathf.Abs(grav) * timeToJumpApex;

                            velocity.y = jumpVel;
                        }
                        else
                        {
                            velocity.y = jumpVelocity;
                        }

                        jump = false;
                    }
                    else if (!controller.collisions.below)
                    {
                        jump = false;
                    }
                }
                
                // Droping down on platforms
                if(canDropDown)
                {
                    if (input.y == -1 && controller.collisions.below)
                    {
                        controller.jumpDown = true;
                    }
                    else if (input.y > 0)
                        controller.jumpDown = false;
                }

                #endregion
            }
        }

        if (Mathf.Abs(target.transform.position.x - transform.position.x) < 0.5f)
            input.x = 0;

        float targetVelocityX = input.x * movementSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (path == null || currentWaypoint >= path.vectorPath.Count || pathIsEnded || dist == 0)
            return;
        
        if(dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
