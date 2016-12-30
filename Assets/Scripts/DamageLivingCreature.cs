using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLivingCreature : MonoBehaviour
{
    public int damage;
    public bool damagePlayer;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<LivingCreature>() != null)
        {
            if (damagePlayer && other.GetComponent<EnemyCreature>() != null)
                return;

            LivingCreature target = other.GetComponent<LivingCreature>();
            
            bool hit = target.Damage(damage, transform.parent.GetComponent<LivingCreature>());

            if(transform.root.GetComponent<PlayerCreature>() != null && hit)
            {
                transform.root.GetComponent<PlayerCreature>().RestoreHealthAfterAttack();
            }
        }
    }
}
