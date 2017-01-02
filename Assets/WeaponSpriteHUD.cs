using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSpriteHUD : MonoBehaviour
{
    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

	void Update ()
    {
        img.sprite = WeaponManager.wp.equippedWeapon.sprite;
	}
}
