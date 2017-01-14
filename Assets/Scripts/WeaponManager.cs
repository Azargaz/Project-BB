using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string Name;
        [HideInInspector]
        public int id; // Don't change! ID used for weapon pedestals        
        public enum AnimationType { horizontal, vertical, command, horizontal_vertical, dash, charging }; // Read below

        [Header("Primary attack")]        
        public int baseDamage;
        [HideInInspector]        
        public int criticalDamage; // Don't change! Assigned automaticaly in Init() function
        public int knockbackPower;                   
        public int useStaminaCost;
        public GameObject[] aoeObject; // 'Swing' object for this weapon, if has combohits it should have more than 1        
        public AnimationType attackType; // Animation used with this weapon
        [Range(0.0f, 100.0f)]
        public float criticalChance;
        public float criticalMultiplier = 1.5f;
        public bool crit; // Used for crits, if true next attack will be critical
        public int comboHits = 0; // Number of additional hits
        public float attackSpeed = 1; // Speed of player's and swings' animations
        public bool chargable;

        [Header("Secondary attack")]
        public int secondaryBaseDamage;
        [HideInInspector]
        public int secondaryCriticalDamage; // Don't change! Assigned automaticaly in Init() function
        public int secondaryKnockbackPower;
        public bool secondaryAttack = false;
        public int secondaryUseStaminaCost;
        public GameObject[] secondaryAoeObject;
        public AnimationType secondaryType; // Animation used with secondary attack of this weapon        
        [Range(0.0f, 100.0f)]
        public float secondaryCriticalChance;
        public float secondaryCriticalMultiplier = 1.5f;
        public bool secondaryCrit; // Used for crits, if true next attack will be critical
        public int secondaryComboHits = 0; // Number of additional hits
        public float secondaryAttackSpeed = 1; // Speed of player's and swings' animations
        public bool secondaryChargable;

        [Header("Other")]
        public Sprite sprite; // Sprite of weapon     

        [Header("Weapon Specials")]
        public bool weaponSpecialActive = false;
        public float weaponSpecialDelay = 0f; // Delay (or lack of it) for weapon special abilities e.g: dagger dash behind enemy

        public void Init()
        {
            criticalDamage = Mathf.RoundToInt(baseDamage * criticalMultiplier);
            secondaryCriticalDamage = Mathf.RoundToInt(secondaryBaseDamage * secondaryCriticalMultiplier);
            crit = false;
            secondaryCrit = false;
        }
    }

    public static WeaponManager wp;
    [HideInInspector]
    public Weapon equippedWeapon;
    [Header("Weapons")]
    public int currentWeapon = 0;
    public Weapon[] weapons;
    [Header("Weapon specific variables")]
    public int daggersDashDistance = 15;
    public int katanaDashDistance = 15;

    public void RollCritical(bool primaryAttack)
    {
        if(primaryAttack)
        {
            equippedWeapon.crit = false;

            float roll = Random.value;

            if ((equippedWeapon.criticalChance / 100f) > roll)
            {
                equippedWeapon.crit = true;
            }
        }
        else
        {
            equippedWeapon.secondaryCrit = false;

            float roll = Random.value;

            if ((equippedWeapon.secondaryCriticalChance / 100f) > roll)
            {
                equippedWeapon.secondaryCrit = true;
            }
        }
    }

    void Awake()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init();
            weapons[i].id = i;
        }

        wp = this;
    }

	void Update ()
    {
        equippedWeapon = weapons[currentWeapon];
	}
}
