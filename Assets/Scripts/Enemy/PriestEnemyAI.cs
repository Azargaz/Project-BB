using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestEnemyAI : GroundEnemyAI
{
    public int healAmount;
    public EnemyHealRadar healRange;
    public LayerMask projectileMask;

    protected override void Update()
    {        
        if (currentState == EnemyState.attack)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position, attackRange, projectileMask);

            int rolledAttack = RollWithWeights(attacks);            

            if (creature.stats.curHealth >= creature.stats.maxHealth)
                rolledAttack = 0;

            if (hit && rolledAttack == 0)
            {
                currentState = EnemyState.walk;
            }

            anim.SetFloat("AttackId", rolledAttack / 10f);
        }

        base.Update();
    }

    void AnimationHeal()
    {
        if(healRange.enemiesInRange.Count > 0)
        {
            for (int i = 0; i < healRange.enemiesInRange.Count; i++)
            {
                if (healRange.enemiesInRange[i] == null)
                {
                    healRange.enemiesInRange.RemoveAt(i);
                    i--;
                    continue;
                }

                healRange.enemiesInRange[i].Heal(healAmount);
            }
        }
    }
}
