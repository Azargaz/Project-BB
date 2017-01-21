using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSpriteHUD : MonoBehaviour
{
    Image img;
    public Image charge;
    public Image weaponType;

    void Awake()
    {
        img = GetComponent<Image>();
    }

	void Update ()
    {
        if(img != null)
        {
            img.sprite = WeaponController.wc.equippedWeapon.sprite;
        }        

        if(weaponType != null)
        {
            weaponType.color = ChooseWeaponNameColor();
            weaponType.color = new Color(weaponType.color.r, weaponType.color.g, weaponType.color.b, 0.7f);
        }

        if(charge != null)
        {
            WeaponController.Weapon.Attack atk = WeaponController.wc.eqWeaponCurAttack;

            if (atk.chargeTime > 0)
                charge.fillAmount = atk.chargeTimer / atk.chargeTime;
            else
                charge.fillAmount = 0;
        }
    }

    Color ChooseWeaponNameColor()
    {
        switch (WeaponController.wc.equippedWeapon.weaponType)
        {
            case WeaponController.Weapon.WeaponType.Strength:
                {
                    return Color.red;
                }
            case WeaponController.Weapon.WeaponType.Dexterity:
                {
                    return Color.green;
                }
            case WeaponController.Weapon.WeaponType.Magic:
                {
                    return new Color(0f, 0.5f, 1f);
                }
            case WeaponController.Weapon.WeaponType.StrDex:
                {
                    return Color.red + Color.green;
                }
            case WeaponController.Weapon.WeaponType.StrMagic:
                {
                    return Color.red + Color.blue;
                }
            case WeaponController.Weapon.WeaponType.DexMagic:
                {
                    return Color.green + Color.blue;
                }
        }

        return Color.white;
    }
}
