using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLivingCreature : MonoBehaviour
{
    public int damage;
    public int poiseDmg;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<LivingCreature>() != null && other.name != transform.root.name)
        {
            LivingCreature target = other.GetComponent<LivingCreature>();

            bool hit = target.Damage(damage, poiseDmg, transform.root.gameObject.GetComponent<LivingCreature>());

            if(transform.root.GetComponent<PlayerCreature>() != null && hit)
            {
                transform.root.GetComponent<PlayerCreature>().RestoreHealthAfterAttack();
            }
        }
    }
}
