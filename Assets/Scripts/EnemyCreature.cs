using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreature : LivingCreature
{
    [SerializeField]
    GameObject hitParticles;
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

    public override void Damage(int damageTaken, bool stun)
    {
        base.Damage(damageTaken, stun);

        if (!stats.stunned && stun)
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
    }

    public override void Kill()
    {
        base.Kill();

        Destroy(gameObject);
    }

    void OnTriggerStay2D(Collider2D other)
    {

    }
}