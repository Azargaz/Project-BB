using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemyRadar : MonoBehaviour
{
    SummonSkeletonAI AI;
    Transform currentEnemy;
    float distToEnemy;
    public float maxDistToEnemy;

    void Awake()
    {
        AI = GetComponentInParent<SummonSkeletonAI>();
        maxDistToEnemy = AI.range;
    }

    void Update()
    {
        if(currentEnemy != null)
        {
            distToEnemy = Vector2.Distance(transform.position, currentEnemy.position);

            if(distToEnemy > maxDistToEnemy)
            {
                currentEnemy = null;
                AI.enemyTarget = null;
                AI.SwitchTarget();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 10 || other.gameObject.layer == 13)
        {
            if(AI.enemyTarget == null)
            {
                AI.enemyTarget = other.transform;
                currentEnemy = other.transform;
                AI.SwitchTarget();
            }
        }
    }
}
