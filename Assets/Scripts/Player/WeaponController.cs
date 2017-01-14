using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string Name; // Weapon name, displayed in inspector as array element's name
        [HideInInspector]
        public int id; // ID for now used only for weapon pedestals, don't touch
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
            [Range(1, 2)]
            public int orderNumber = 1; // Primary or Secondary attack, allows to add more attack for future different weapons
            [Range(0, 999)]
            public int staminaCost;
            [Range(0, 999)]
            public int baseDamage; // What did you expect in this comment?
            [HideInInspector]
            public int criticalDamage; // Don't change this directly
            [Range(0, 100)]
            public int criticalChance; // Chance for critical strike
            public bool crit; // Don't change this directly, determines wheter next attack is a critical or not           
            [Range(1, 100)]
            public int numberOfHits = 1; // If weapon hits 2 or more times with single strike, change this variable to whatever is the number of hits per strike
            [Range(-100f, 100f)]
            public float attackSpeed = 1; // Speed of attack animations
            public AttackAnimations type; // Type of animation used with this attack
            public bool chargable = false; // Can this attack be charged?
            public Animator[] aoeAnim;
        }

        ///////////////////////////////////////////////
    }
}
