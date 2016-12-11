using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage;
    public int ticksOfDamage = 0;
    public float damageInterval;
    public bool stunPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerCreature>() != null)
        {
            PlayerCreature player = other.GetComponent<PlayerCreature>();

            if (ticksOfDamage > 0)
                StartCoroutine(player.DamageOverTime(ticksOfDamage, damage, damageInterval, stunPlayer));
            else
                player.Damage(damage, stunPlayer);
        }
    }
}
