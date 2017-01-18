using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string Name;
        [HideInInspector]
        public int id; // ID for now used only for weapon pedestals, don't touch
        public Sprite sprite;
        public enum WeaponType { Strength, Dexterity, Magic, StrDex, StrMagic, DexMagic };
        public WeaponType weaponType;
        [SerializeField]
        public Attack[] attacks; // How many attacks does this weapon have, and all of their properties. For only primary and secondary attacks

        ///////////////////////////////////////////////////////

        /* Different animation types for different attacks. 
        For now it's possible to have 11 different animations.
        To add more space for animations:
        - in Player script there is: currentWeapon.currentAttack.type / value - change this value to add more space for animations */
        public enum AttackAnimations{ horizontal, vertical, command, horizontal_then_vertical, dashing, charging };

        ///////////////////////////////////////////////////////


        ///////////////////////////////////////////////
        
        [System.Serializable]
        public class Attack
        {                 
            public string attackOrder; // Primary or Secondary attack
            /////////////////////////////////
            /* NORMAL ATTACK */
            [Header("Normal attack")]
            [Range(0, 999)]
            public int baseDamage;
            [HideInInspector]
            public int initialBaseDamage;
            [Range(0, 999)]
            public int knockbackPower;
            [Range(0, 999)]
            public int staminaCost;
            /////////////////////////////////

            /////////////////////////////////
            /* CRITICAL DAMAGE STATS */
            [Header("Critical damage")]
            [HideInInspector]
            public int criticalDamage; // Don't change this directly
            [Range(0, 100)]
            public int criticalChance; // Chance for critical strike
            [Range(0, 999)]
            public float criticalMultiplier; // Chance for critical strike
            public bool crit; // Don't change this directly, determines wheter next attack is a critical or not   

            [Header("Combo, attack speed, other")]
            [Range(1, 100)]
            public int numberOfHits = 1; // If weapon hits 2 or more times with single strike, change this variable to whatever is the number of hits per strike
            public AttackAnimations[] animationTypes; // Type of animation used with this attack            
            public Animator[] chargeAnim;
            public Animator[] aoeAnim;
            [Range(0f, 2f)]
            public float attackSpeed = 1; // Speed of attack animations
            /////////////////////////////////

            /////////////////////////////////
            /* CHARGE ATTACK */
            [Header("Charged attack")]
            public bool chargable = false; // Can this attack be charged?
            public bool needFullCharge = false; // Do you need to charge this attack for full time?
            public bool scaleDamageWithChargeTime = false; // Scale damage with how long player has charged, works only if full charge isn't needed
            [Range(0, 5)]
            public float chargeTime = 1f; // How long does player have to hold button for attack to be fully charged     
            [HideInInspector]
            public float chargeTimer = 0f; // How long has this weapon been charged?       
            [Range(0, 999)]
            public int chargedDamage;
            [HideInInspector]
            public int initialChargedDamage;
            [Range(0, 999)]
            public int chargedKnockbackPower;
            [Range(0, 999)]
            public int chargedStaminaCost;
            /////////////////////////////////

            /////////////////////////////////
            /* DASH WITH ATTACK */
            [Header("Dash")]
            public bool dashWithAttack = false; // For attack with dash
            public enum DashDirection { normal, reversed };
            public DashDirection dashDirection;
            public bool dashFacingBackwards = false; // Change to true if player should change facing direction after dash
            public float dashDistance = 10f; // Dash distance      
            /////////////////////////////////      
        }

        ///////////////////////////////////////////////

        [HideInInspector]
        public bool restoreHealthMechanic = false;

        public void Init()
        {
            if(Name.Contains("Scythe"))
                restoreHealthMechanic = true;

            for (int i = 0; i < attacks.Length; i++)
            {
                Attack a = attacks[i];
                a.attackOrder = (i) == 0 ? "Primary" : "Secondary";
                a.criticalDamage = Mathf.RoundToInt(a.baseDamage * a.criticalMultiplier);
                a.crit = false;

                a.initialBaseDamage = a.baseDamage;
                a.initialChargedDamage = a.chargedDamage;

                if(!a.chargable)
                {
                    a.chargeTime = 0;
                }

                if(!a.dashWithAttack)
                {
                    a.dashDistance = 0;
                }
            }
        }

        public void RefreshCriticalDmg()
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                Attack a = attacks[i];
                a.criticalDamage = Mathf.RoundToInt(a.baseDamage * a.criticalMultiplier);
            }
        }

        public void Use()
        {
            switch(id)
            {
                case 0:
                    {
                        
                        break;
                    }
            }
        }

        public void MultiplyDamageByBonus(float multiplier)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                attacks[i].baseDamage = (int)(attacks[i].initialBaseDamage * multiplier);
                attacks[i].chargedDamage = (int)(attacks[i].initialChargedDamage * multiplier);                
            }

            RefreshCriticalDmg();
        }
    }

    public static WeaponController wc;
    [HideInInspector]
    public Weapon equippedWeapon;
    [HideInInspector]
    public Weapon.Attack eqWeaponCurAttack;

    public int currentWeapon = 0;
    public int currentAttack = 0;

    [Range(1f, 10f)]
    public float strengthDamageBonus = 1;
    [Range(1f, 10f)]
    public float dexterityDamageBonus = 1;
    [Range(1f, 10f)]
    public float magicDamageBonus = 1;

    [SerializeField]
    public Weapon[] weapons;

    public void RollCritical(int attackNumber)
    {
        equippedWeapon.attacks[attackNumber].crit = false;

        float roll = Random.value;

        if ((equippedWeapon.attacks[attackNumber].criticalChance / 100f) > roll)
        {
            equippedWeapon.attacks[attackNumber].crit = true;
        }        
    }

    void Awake()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init();
            weapons[i].id = i;
        }

        currentAttack = 0;
        wc = this;
    }

    void Update()
    {
        equippedWeapon = weapons[currentWeapon];

        if (currentAttack > equippedWeapon.attacks.Length - 1)
            currentAttack = 0;

        eqWeaponCurAttack = equippedWeapon.attacks[currentAttack];

        WeaponTypeDamageBonuses();
    }

    void WeaponTypeDamageBonuses()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            float multiplier = 1;

            switch(weapons[i].weaponType)
            {
                case Weapon.WeaponType.Strength:
                    {
                        multiplier = strengthDamageBonus;
                        break;
                    }
                case Weapon.WeaponType.Dexterity:
                    {
                        multiplier = dexterityDamageBonus;
                        break;
                    }
                case Weapon.WeaponType.Magic:
                    {
                        multiplier = magicDamageBonus;
                        break;
                    }
                case Weapon.WeaponType.StrDex:
                    {
                        multiplier = (strengthDamageBonus + dexterityDamageBonus) / 2f;
                        break;
                    }
                case Weapon.WeaponType.StrMagic:
                    {
                        multiplier = (strengthDamageBonus + magicDamageBonus) / 2f;
                        break;
                    }
                case Weapon.WeaponType.DexMagic:
                    {
                        multiplier = (dexterityDamageBonus + magicDamageBonus) / 2f;
                        break;
                    }
            }

            weapons[i].MultiplyDamageByBonus(multiplier);
        }
    }
}
