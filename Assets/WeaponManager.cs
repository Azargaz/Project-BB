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
        public Sprite sprite;
        public GameObject aoeObject;
        public enum AnimationType { vertical, horizontal, command };
        public AnimationType attackType;
    }

    public static WeaponManager wp;
    public Weapon equippedWeapon;
    public Weapon[] weapons;
    public int currentWeapon = 0;
    public Image weaponSpriteHUD;

    void Awake()
    {
        wp = this;
    }

	void Update ()
    {
		if(weaponSpriteHUD != null && weapons[currentWeapon].sprite != null)
        {
            weaponSpriteHUD.sprite = weapons[currentWeapon].sprite;
        }

        equippedWeapon = weapons[currentWeapon];
	}
}
