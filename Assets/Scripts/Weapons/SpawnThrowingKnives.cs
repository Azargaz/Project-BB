using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnThrowingKnives : SpawnProjectile
{
    [SerializeField]
    int numberOfKnivesCharged;
    int numberOfKnives;

    protected override void AnimationSpawnProjectile()
    {
        if (projectileObject == null)
            return;

        if (PlayerWeaponController.fullyCharged)
            numberOfKnives = numberOfKnivesCharged;
        else
            numberOfKnives = 1;

        for (int i = 0; i < numberOfKnives; i++)
        {
            SpawnKnife();
        }
    }

    void SpawnKnife()
    {
        firePoint.localPosition = new Vector2(firePointInitialPos.x + Random.Range(-firePointRandomOffsetX, firePointRandomOffsetX), firePointInitialPos.y + Random.Range(-firePointRandomOffsetY, 0));

        GameObject clone = Instantiate(projectileObject, firePoint.transform.position, Quaternion.identity);
        Projectile projectile = clone.GetComponent<Projectile>();
        clone.transform.localScale = new Vector2(transform.parent.localScale.x, Random.value > 0.5f ? 1 : -1);

        if (transform.root.GetComponent<LivingCreature>() != null)
        {
            projectile.creature = transform.root.GetComponent<LivingCreature>();
        }
        else if (transform.parent.GetComponent<LivingCreature>() != null)
        {
            projectile.creature = transform.parent.GetComponent<LivingCreature>();
        }

        projectile.input.x = transform.parent.localScale.x;
    }
}
