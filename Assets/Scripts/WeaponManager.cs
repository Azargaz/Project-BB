using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        [HideInInspector]
        public int id; // Don't change! ID used for weapon pedestals
        public string name;
        public int baseDamage;
        [HideInInspector]        
        public int criticalDamage; // Don't change! Assigned automaticaly in Init() function
        public int knockbackPower;
        public int useStaminaCost;
        [Range(0.0f, 100.0f)]
        public float criticalChance;
        public float criticalMultiplier = 1.5f;
        public bool crit; // Used for crits, if true next attack will be critical
        public Sprite sprite; // Sprite of weapon
        public GameObject[] aoeObject; // 'Swing' object for this weapon, if has combohits it should have more than 1
        public enum AnimationType { horizontal, vertical, command, horizontal_vertical, dash }; // Read below
        public AnimationType attackType; // Animation used with this weapon
        public int comboHits = 0; // Number of additional hits
        public float attackSpeed = 1; // Speed of player's and swings' animations
        [Header("Weapon Specials")]
        public bool weaponSpecialActive = false;
        public float weaponSpecialDelay = 0f; // Delay (or lack of it) for weapon special abilities e.g: dagger dash behind enemy

        public void Init()
        {
            criticalDamage = Mathf.RoundToInt(baseDamage * criticalMultiplier);
            crit = false;
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

    public void RollCritical()
    {
        equippedWeapon.crit = false;

        float roll = Random.value;
        
        if((equippedWeapon.criticalChance / 100f) > roll)
        {
            equippedWeapon.crit = true;
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
