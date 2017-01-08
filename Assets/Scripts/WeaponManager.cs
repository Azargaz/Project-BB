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
        public int id;
        public string name;
        public int baseDamage;        
        public int criticalDamage;
        public int knockbackPower;
        public int useStaminaCost;
        [Range(0.0f, 100.0f)]
        public float criticalChance;
        public float criticalMultiplier = 1.5f;
        public bool crit;
        public Sprite sprite;
        public GameObject[] aoeObject;
        public enum AnimationType { horizontal, vertical, command, horizontal_vertical, dash };
        public AnimationType attackType;
        public int comboHits;
        public float attackSpeed = 1;

        public void Init()
        {
            criticalDamage = (int)(baseDamage * criticalMultiplier);
            crit = false;
        }
    }

    public static WeaponManager wp;
    [HideInInspector]
    public Weapon equippedWeapon;
    public int currentWeapon = 0;
    public Weapon[] weapons;     

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
