using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLivingCreature : MonoBehaviour
{            
    public enum Target { enemy, player, all };
    [Header("Damage living creatures")]
    public Target target;
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

        LivingCreature _target = other.GetComponent<LivingCreature>();

        if (!_target.stats.alive)
            return;

        if (_target == creature)
            return;

        if ((_target is PlayerCreature || _target is SummonCreature) && target == Target.enemy)
            return;

        if (_target is EnemyCreature && target == Target.player)
            return;

        bool hit = _target.Damage(damage, creature, creature == null ? 0 : creature.stats.knockbackPower);

        if (hit)
            AfterHit(other.gameObject);
    }

    protected virtual void AfterHit(GameObject targetHit)
    {
        if (creature is PlayerCreature)
        {
            PlayerCreature playerCreature = creature as PlayerCreature;
            playerCreature.RestoreHealthAfterAttack();
        }
    }
}
