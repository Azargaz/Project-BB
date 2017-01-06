using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    protected enum EnemyState { idle, stop, walk, attack };
    protected EnemyState currentState = EnemyState.idle;

    public float searchPlayerInterval = 0.05f;
    float searchPlayerDelay = 0;
    public float range = 10f;
    public float attackRange = 1f;
    public float followRange = 1f;
    public float attackCooldown = 1f;
    float attackTimer;
    public float minDistanceFromPlayer = 0.5f;

    [HideInInspector]
    public Vector3 velocity;

    GameObject player;
    protected Vector2 playerDirection = Vector2.zero; 
    protected Vector2 playerPos;

    [Header("")]
    bool debugAI;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform.position;
        attackTimer = attackCooldown;        
    }

    protected virtual void Update()
    {
        currentState = SearchPlayer();

        // Attack & SearchPlayer cooldown
        attackTimer -= Time.deltaTime;
        searchPlayerDelay -= Time.deltaTime;
    }

    protected EnemyState SearchPlayer()
    {
        // If no player return
        if (player == null)
        {
            if (debugAI)
                Debug.LogError("GroundEnemyAI can't find player.");
            return EnemyState.idle;
        }

        if (searchPlayerDelay <= 0)
        {
            playerPos = player.transform.position;
            searchPlayerDelay = searchPlayerInterval;
        }

        Vector2 playerPosDifference = (Vector3)playerPos - transform.position;
        float distanceToPlayer = Vector2.Distance(playerPos, transform.position);

        // If player is out of range return
        if (distanceToPlayer > range)
        {
            if (debugAI)
                Debug.Log("Player out of range.");
            return EnemyState.idle;
        }

        // If player is in attack range, attack - if it's on cooldown, stop
        if (attackTimer <= 0)
        {
            if (distanceToPlayer <= attackRange)
            {
                playerDirection = new Vector2(playerPosDifference.x > 0 ? 1 : -1, playerPosDifference.y > 0 ? 1 : -1);
                attackTimer = attackCooldown;
                return EnemyState.attack;
            }
            else
            {             
                if (Mathf.Abs(playerPosDifference.x) > minDistanceFromPlayer)
                {
                    playerDirection = new Vector2(playerPosDifference.x > 0 ? 1 : -1, playerPosDifference.y > 0 ? 1 : -1);
                    return EnemyState.walk;
                }

                return EnemyState.stop;
            }
        }
        // Else walk towards player
        else
        {
            if(distanceToPlayer > followRange && Mathf.Abs(playerPosDifference.x) > minDistanceFromPlayer)
            {
                playerDirection = new Vector2(playerPosDifference.x > 0 ? 1 : -1, playerPosDifference.y > 0 ? 1 : -1);
                return EnemyState.walk;
            }            
            // If player is too close to enemy, stop          
            else
            {
                return EnemyState.stop;
            }
        }
    }
}
