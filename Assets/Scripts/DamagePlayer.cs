using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage;
    public int poiseDmg;
    public int ticksOfDamage = 0;
    public float damageInterval;
    public bool stunPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<LivingCreature>() != null)
        {
            LivingCreature player = other.GetComponent<LivingCreature>();

            player.Damage(damage, poiseDmg, transform.root.gameObject.GetComponent<LivingCreature>());
        }
    }
}
