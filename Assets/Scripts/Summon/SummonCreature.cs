using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCreature : LivingCreature
{
    [SerializeField]
    GameObject hitParticles;
    [SerializeField]
    GameObject deathParticles;
    Animator anim;
    EnemyAI controller;    

    void Awake()
    {
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<EnemyAI>();
    }

    public override bool Damage(int damageTaken, LivingCreature dmgSource, int knockbackPower)
    {
        base.Damage(damageTaken, dmgSource, knockbackPower);

        if (stats.invincible)
            return false;

        if (!stats.stunned)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");

            if (controller != null)
                controller.attacked = false;

            stats.stunned = true;
        }

        if (hitParticles != null)
        {
            GameObject clone = Instantiate(hitParticles, transform.position, transform.localRotation, transform);
            Destroy(clone, 3f);
        }

        // Knockback
        if (knockbackPower != 0 && dmgSource != null && controller != null && !stats.immovable && !controller.freeze)
        {
            float direction = Mathf.Sign(dmgSource.transform.position.x - transform.position.x);
            float velocityX = (direction > 0 ? -1 : 1) * knockbackPower;
            controller.velocity.x += velocityX;
        }

        return true;
    }

    public override void Kill()
    {
        base.Kill();

        if (deathParticles != null)
        {
            GameObject clone = Instantiate(deathParticles, transform.position, transform.localRotation);
            Destroy(clone, 3f);
        }

        if (anim != null)
            anim.SetTrigger("Death");
        else
            Destroy(gameObject);
    }
}
