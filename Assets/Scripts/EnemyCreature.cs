using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreature : LivingCreature
{
    [SerializeField]
    GameObject hitParticles;
    [SerializeField]
    GameObject deathParticles;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        stats.Initialize();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Damage(int damageTaken, bool stun, int poiseDamage)
    {
        base.Damage(damageTaken, stun, poiseDamage);

        if (!stats.stunned && stun && stats.poise <= 0)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");            

            stats.stunned = true;
            stats.poise = stats.maxPoise;
        }

        if (hitParticles != null)
        {
            GameObject clone = Instantiate(hitParticles, transform.position, transform.localRotation, transform);
            Destroy(clone, 3f);
        }
    }

    public override void Kill()
    {
        base.Kill();

        if (deathParticles != null)
        {
            GameObject clone = Instantiate(deathParticles, transform.position, transform.localRotation);
            Destroy(clone, 3f);
        }

        GameManager.Score++;
        Destroy(gameObject);
    }
}