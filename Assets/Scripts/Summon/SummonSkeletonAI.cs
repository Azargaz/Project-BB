using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class SummonSkeletonAI : GroundEnemyAI
{
    [Header("Summon Skeleton AI")]
    public float minDistanceFromPlayer = 1f;
    float initialRange;
    float initialMinDistanceFromTarget;

    protected override void Start()
    {
        base.Start();

        initialRange = range;
        initialMinDistanceFromTarget = minDistanceFromTarget;
        SwitchTarget();
    }

    public void SwitchTarget()
    {
        if (enemyTarget != null)
        {
            canAttack = true;
            range = initialRange;
            minDistanceFromTarget = initialMinDistanceFromTarget;          
        }
        else
        {
            canAttack = false;
            range = 100;
            minDistanceFromTarget = minDistanceFromPlayer;
        }
    }

    protected override void Update()
    {
        base.Update();

        anim.SetBool("Grounded", controller.collisions.below);

        if (enemyTarget == null)
            SwitchTarget();
    }
}
