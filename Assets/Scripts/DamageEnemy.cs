using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    public int damage;
    public int poiseDmg;
    public int ticksOfDamage = 0;
    public float damageInterval;
    public bool stunEnemy;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyCreature>() != null)
        {
            EnemyCreature player = other.GetComponent<EnemyCreature>();

            player.Damage(damage, poiseDmg, transform.root.gameObject.GetComponent<LivingCreature>());
        }
    }
}
