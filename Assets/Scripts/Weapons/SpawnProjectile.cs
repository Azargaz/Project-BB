using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectile : MonoBehaviour
{ 
    public GameObject projectileObject;
    public Transform firePoint;
    public float firePointRandomOffsetX;
    public float firePointRandomOffsetY;
    protected Vector2 firePointInitialPos;

    protected void Awake()
    {
        firePointInitialPos = firePoint.localPosition;
    }

    public virtual void AnimationSpawnProjectile()
    {
        if (projectileObject == null)
            return;

        firePoint.localPosition = new Vector2(firePointInitialPos.x + Random.Range(-firePointRandomOffsetX, firePointRandomOffsetX), firePointInitialPos.y + Random.Range(-firePointRandomOffsetY, firePointRandomOffsetY));

        GameObject clone = Instantiate(projectileObject, firePoint.transform.position, Quaternion.identity);
        Projectile projectile = clone.GetComponent<Projectile>();
        clone.transform.localScale = new Vector2(transform.parent.localScale.x, 1);

        if (transform.parent.GetComponent<EnemyCreature>() != null)
            clone.transform.localScale = new Vector2(transform.localScale.x, 1);

        if (transform.root.GetComponent<LivingCreature>() != null)
        {
            projectile.creature = transform.root.GetComponent<LivingCreature>();
        }
        else if (transform.parent.GetComponent<LivingCreature>() != null)
        {
            projectile.creature = transform.parent.GetComponent<LivingCreature>();
        }

        projectile.input.x = transform.parent.localScale.x;

        if (transform.parent.GetComponent<EnemyCreature>() != null)
            projectile.input.x = transform.localScale.x;
    }
}
