using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPedestalController : MonoBehaviour
{
    public int weaponId;
    int weaponIdStored;
    public SpriteRenderer weaponSprite;

	void Start ()
    {
        weaponId = Random.Range(0, 2);
	}

    void Update()
    {
        if(weaponSprite != null && WeaponManager.wp.weapons[weaponId] != null)
        {
            weaponSprite.sprite = WeaponManager.wp.weapons[weaponId].sprite;
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            weaponIdStored = other.GetComponentInChildren<WeaponManager>().currentWeapon;
            other.GetComponentInChildren<WeaponManager>().currentWeapon = weaponId;
            weaponId = weaponIdStored;
        }
    }
}
