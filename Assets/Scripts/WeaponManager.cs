using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public int damage;
        public int knockbackPower;
        public int useStaminaCost;
        [Range(0.0f, 100.0f)]
        public float criticalChance;
        public Sprite sprite;
        public GameObject[] aoeObject;
        public enum AnimationType { vertical, horizontal, command, vertical_horizontal };
        public AnimationType attackType;
    }

    public static WeaponManager wp;
    [HideInInspector]
    public Weapon equippedWeapon;
    public int currentWeapon = 0;
    public Weapon[] weapons;     

    void Awake()
    {
        wp = this;
    }

	void Update ()
    {
        equippedWeapon = weapons[currentWeapon];
	}
}
