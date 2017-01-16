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
            [Range(0, 999)]
            public int baseDamage;
            [Range(0, 999)]
            public int knockbackPower;
            [Range(0, 999)]
            public int staminaCost;        
            
            [HideInInspector]
            public int criticalDamage; // Don't change this directly
            [Range(0, 100)]
            public int criticalChance; // Chance for critical strike
            [Range(0, 999)]
            public float criticalMultiplier; // Chance for critical strike
            public bool crit; // Don't change this directly, determines wheter next attack is a critical or not   
                    
            [Range(1, 100)]
            public int numberOfHits = 1; // If weapon hits 2 or more times with single strike, change this variable to whatever is the number of hits per strike
            public AttackAnimations[] type; // Type of animation used with this attack            
            public Animator[] chargeAnim;
            public Animator[] aoeAnim;
            [Range(0f, 2f)]
            public float attackSpeed = 1; // Speed of attack animations
            
            public bool chargable = false; // Can this attack be charged?
            public float chargeTime = 1f; // How long does player have to hold button for attack to be fully charged
            public bool dashWithAttack = false; // For attack with dash
            public enum DashDirection { normal, reversed };
            public DashDirection dashDirection;
            public bool dashFacingBackwards = false; // Change to true if player should change facing direction after dash
            public float dashDistance = 10f; // Dash distance            
        }

        ///////////////////////////////////////////////

        public void Init()
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                Attack a = attacks[i];
                a.attackOrder = (i) == 0 ? "Primary" : "Secondary";
                a.criticalDamage = Mathf.RoundToInt(a.baseDamage * a.criticalMultiplier);
                a.crit = false;

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
    }

    public static WeaponController wc;
    [HideInInspector]
    public Weapon equippedWeapon;
    [HideInInspector]
    public Weapon.Attack eqWeaponCurAttack;
    public int currentWeapon = 0;
    public int currentAttack = 0;
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

        if (currentAttack > equippedWeapon.attacks.Length)
            currentAttack = 0;

        eqWeaponCurAttack = equippedWeapon.attacks[currentAttack];
    }
}
