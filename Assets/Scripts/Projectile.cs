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

    float velocityXSmoothing;
    Vector2 velocity;
    Controller2D controller;
    [HideInInspector]
    public GameObject source;
    public bool freeze;

    protected override void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    void Start()
    {
        GameManager.instance.flyingProjectiles.Add(gameObject);

        damage = creature.stats.damage;
    }

    protected override void Update()
    {
        if (lifeTime <= 0)
            Destroy(gameObject);

        if (freeze)
            return;

        lifeTime -= Time.deltaTime;

        if (controller.collisions.right || controller.collisions.left)
            Destroy(gameObject);

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 0.1f);

        controller.Move(velocity * Time.deltaTime);
    }

    protected override void AfterHit()
    {
        if (source != null)
        {
            source.GetComponent<PlayerCreature>().RestoreHealthAfterAttack();
        }

        Destroy(gameObject);
    }
}
