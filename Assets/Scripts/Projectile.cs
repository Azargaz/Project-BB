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
    public bool freeze;
    public bool pause;

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
        if (pause)
            return;

        if (lifeTime <= 0)
            Destroy(gameObject);

        if (freeze)
        {
            storedVelocity = velocity;
            velocity = Vector3.Lerp(storedVelocity, Vector3.zero, freezeSmoothTime);
            freezeSmoothTime += Time.deltaTime / 10000f;
            controller.Move(velocity * Time.deltaTime);
            return;
        }

        lifeTime -= Time.deltaTime;

        if (controller.collisions.right || controller.collisions.left)
            Destroy(gameObject);

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);

        controller.Move(velocity * Time.deltaTime);
    }

    protected override void AfterHit()
    {
        base.AfterHit();

        Destroy(gameObject);
    }
}
