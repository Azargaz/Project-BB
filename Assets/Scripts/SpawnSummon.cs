using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSummon : MonoBehaviour
{
    public int maxSummons = 10;
    public Transform spawnPoint;
    public GameObject summon;
    List<GameObject> summons = new List<GameObject>();

    void Update()
    {
        if(summons.Count > 0 && summons[0] == null)
        {
            summons.RemoveAt(0);
        }

        if(summons.Count > maxSummons)
        {
            summons[0].GetComponent<LivingCreature>().Kill();
            summons.RemoveAt(0);
        }
    }

	void AnimationSummon()
    {
        GameObject clone = Instantiate(summon, spawnPoint.position, Quaternion.identity);
        LivingCreature.Statistics stats = clone.GetComponent<SummonCreature>().stats;
        WeaponController.Weapon.Attack atk = WeaponController.wc.eqWeaponCurAttack;
        stats.damage = atk.baseDamage;

        if (atk.crit)
        {
            Color swingColor = clone.GetComponent<SpriteRenderer>().color;
            swingColor = new Color(1f, 0.5f, 0.5f);
            clone.GetComponent<SpriteRenderer>().color = swingColor;

            stats.maxHealth = (int)(atk.criticalMultiplier * stats.maxHealth);
            stats.Initialize();            
            stats.damage = atk.criticalDamage;            
        }

        clone.transform.parent = GameObject.Find("PlayerSummons").transform;
        summons.Add(clone);
    }
}
