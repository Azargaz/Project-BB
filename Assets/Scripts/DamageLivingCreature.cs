using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLivingCreature : MonoBehaviour
{
    public int damage;
    public bool damagePlayer;
    PlayerCreature player;

    void Awake()
    {
        if (transform.root.GetComponent<PlayerCreature>() != null)
        {
            player = transform.root.GetComponent<PlayerCreature>();
        }
    }

    void Update()
    {
        if(player != null)
        {
            damage = player.weaponM.weapons[player.weaponM.currentWeapon].damage;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<LivingCreature>() != null)
        {
            if (damagePlayer && other.GetComponent<EnemyCreature>() != null)
                return;

            LivingCreature target = other.GetComponent<LivingCreature>();
            LivingCreature thisObject;

            if (transform.parent.GetComponent<LivingCreature>() != null)
                thisObject = transform.parent.GetComponent<LivingCreature>();
            else if (transform.GetComponent<LivingCreature>() != null)
                thisObject = transform.GetComponent<LivingCreature>();
            else
                thisObject = transform.root.GetComponent<LivingCreature>();

            bool hit = target.Damage(damage, thisObject, thisObject == null ? 0 : thisObject.stats.knockbackPower);

            if(transform.root.GetComponent<PlayerCreature>() != null && hit)
            {
                transform.root.GetComponent<PlayerCreature>().RestoreHealthAfterAttack();
            }
        }
    }
}
