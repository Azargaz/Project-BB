using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : EnemyCreature
{
    protected override void Start()
    {
    }

    public override void Kill()
    {
        CurrencyController.CC.AddCurrency(currency);

        if (dropLoot && Random.value <= chanceToDropLoot / 100f)
        {
            GameObject clone = LootController.LC.RollLoot(maxLootRarity);

            if (clone != null)
                Instantiate(clone, transform.position, Quaternion.identity, transform.Find("Loot"));
        }

        Destroy(gameObject);
    }
}
