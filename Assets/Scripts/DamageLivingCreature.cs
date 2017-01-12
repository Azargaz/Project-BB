using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLivingCreature : MonoBehaviour
{        
    [Header("Damage living creatures")]
    public bool damagePlayer;
    public bool damageAll;
    public bool ignoreFlying;

    public int damage;
    [HideInInspector]
    public LivingCreature creature;

    protected virtual void Awake()
    {
        if (transform.root.GetComponent<LivingCreature>() != null)
        {
            creature = transform.root.GetComponent<LivingCreature>();
        }
        else if(transform.parent.GetComponent<LivingCreature>() != null)
        {
            creature = transform.parent.GetComponent<LivingCreature>();
        }
    }

    protected virtual void Update()
    {
        if(creature != null)
        {
            damage = creature.stats.damage;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (ignoreFlying && other.gameObject.layer == 13)
            return;

        if (other.GetComponent<LivingCreature>() == null)
            return;

        LivingCreature target = other.GetComponent<LivingCreature>();

        if (target == creature)
            return;

        if ((target is PlayerCreature || target is SummonCreature) && !damagePlayer && !damageAll)
            return;

        if (target is EnemyCreature && damagePlayer && !damageAll)
            return;

        bool hit = target.Damage(damage, creature, creature == null ? 0 : creature.stats.knockbackPower);

        if (hit)
            AfterHit();
    }

    protected virtual void AfterHit()
    {
        if (creature is PlayerCreature)
        {
            PlayerCreature playerCreature = creature as PlayerCreature;
            playerCreature.RestoreHealthAfterAttack();
        }
    }
}
