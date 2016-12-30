﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreature : LivingCreature
{
    [SerializeField]
    GameObject hitParticles;
    [SerializeField]
    GameObject deathParticles;
    Animator anim;
    GroundEnemyAI controller;

    void Awake()
    {
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<GroundEnemyAI>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool Damage(int damageTaken, LivingCreature dmgSource)
    {
        base.Damage(damageTaken, dmgSource);

        if (stats.invincible)
            return false;

        if (!stats.stunned)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");            

            stats.stunned = true;
        }

        if (hitParticles != null)
        {
            GameObject clone = Instantiate(hitParticles, transform.position, transform.localRotation, transform);
            Destroy(clone, 3f);
        }

        // Knockback
        if (stats.knockbackDistance != 0 && dmgSource != null)
        {
            float direction = Mathf.Sign(dmgSource.transform.position.x - transform.position.x);
            float velocityX = (direction > 0 ? -1 : 1) * stats.knockbackDistance;
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

        GameManager.Score++;
        Destroy(gameObject);
    }
}