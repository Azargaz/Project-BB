using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Projectile : DamageLivingCreature
{
    [Header("Projectile")]
    public Vector2 input;
    public float moveSpeed;
    public float lifeTime;
    public float accelerationTime;
    float velocityXSmoothing;
    Vector2 velocity;
    Vector2 storedVelocity;
    Controller2D controller;
    float freezeSmoothTime = 0.05f;
    public bool pierce = false;
    public bool freeze;
    public bool pause;

    Animator anim;

    protected override void Awake()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        GameManager.instance.flyingProjectiles.Add(gameObject);

        damage = creature.stats.damage;
    }

    protected override void Update()
    {
        if (pause)
            return;

        if (lifeTime <= 0)
            Destroy(gameObject);

        if (freeze)
        {
            if(anim != null)
                anim.speed = Mathf.Lerp(anim.speed, 0, freezeSmoothTime);

            storedVelocity = velocity;
            velocity = Vector3.Lerp(storedVelocity, Vector3.zero, freezeSmoothTime);
            freezeSmoothTime += Time.deltaTime / 10000f;
            controller.Move(velocity * Time.deltaTime);
            return;
        }
        else
        {
            if(anim != null)
                anim.speed = Mathf.Lerp(anim.speed, 1, freezeSmoothTime);
        }

        lifeTime -= Time.deltaTime;

        if (controller.collisions.right || controller.collisions.left)
            Destroy(gameObject);

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        controller.Move(velocity * Time.deltaTime);
    }

    protected override void AfterHit(GameObject targetHit)
    {
        base.AfterHit(targetHit);

        if(target == Target.enemy)
        {
            if (targetHit.layer == 9 || (!pierce && (targetHit.layer == 10 || targetHit.layer == 13)))
            {
                Destroy(gameObject);
            }
        }

        if(target == Target.player)
        {
            if (targetHit.layer == 9 || (!pierce && (targetHit.layer == 8 || targetHit.layer == 15)))
            {
                Destroy(gameObject);
            }
        }
    }
}
