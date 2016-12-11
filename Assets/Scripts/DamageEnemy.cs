using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    public int damage;
    public int ticksOfDamage = 0;
    public float damageInterval;
    public bool stunEnemy;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyCreature>() != null)
        {
            EnemyCreature player = other.GetComponent<EnemyCreature>();

            if (ticksOfDamage > 0)
                StartCoroutine(player.DamageOverTime(ticksOfDamage, damage, damageInterval, stunEnemy));
            else
                player.Damage(damage, stunEnemy);
        }
    }
}
