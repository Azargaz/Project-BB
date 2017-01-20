using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPedestal : MonoBehaviour
{
    Text promptText;
    Text promptWeaponName;
    public int weaponId;
    int weaponIdStored;
    public bool playerHasChosenWeapon;
    [HideInInspector]
    public bool rerollPedestal;
    public Sprite rerollSprite;
    public GameObject notEnoughCurrencyDisplay;

    GameObject player;
    public GameObject prompt;
    public SpriteRenderer weaponPedestalSpriteRenderer;

    void Start ()
    {
        WeaponPedestalController.WPC.weaponPedestals.Add(this);
        promptText = prompt.transform.FindChild("Text").GetComponent<Text>();
        promptWeaponName = prompt.transform.FindChild("WeaponName").GetComponent<Text>();
    }

    void Update ()
    {
        if (!rerollPedestal && weaponPedestalSpriteRenderer != null && WeaponController.wc.weapons[weaponId] != null && prompt != null && promptText != null)
        {
            weaponPedestalSpriteRenderer.sprite = WeaponController.wc.weapons[weaponId].sprite;
            promptText.text = (playerHasChosenWeapon ? "" : "Choose one weapon and defeat powerful monster to escape\n") + "Press Q to pickup";
            promptWeaponName.text = WeaponController.wc.weapons[weaponId].Name;

            Color weaponNameColor = ChooseWeaponNameColor();
            promptWeaponName.color = weaponNameColor;
        }
        else if(rerollPedestal && weaponPedestalSpriteRenderer != null && prompt != null && promptText != null)
        {
            if(rerollSprite != null)
                weaponPedestalSpriteRenderer.sprite = rerollSprite;
            promptText.text = "Reroll for three other weapons\nCost: " + CurrencyController.CC.GetRerollCost() + "$\nPress Q to reroll.";
            promptWeaponName.text = "";
        }

        if (player != null)
        {
            if (Input.GetButtonDown("Interact") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
            {
                SwapWeapon();
            }
        }
    }

    void SwapWeapon()
    {
        if (player == null)
            return;

        if(!rerollPedestal)
        {
            WeaponPedestalController.WPC.Activate(this);
            weaponIdStored = WeaponController.wc.currentWeapon;
            WeaponController.wc.currentWeapon = weaponId;
            weaponId = weaponIdStored;
        }
        else
        {
            if(CurrencyController.CC.RerollWeapons())
                WeaponPedestalController.WPC.RerollWeapons();
            else
            {
                if(notEnoughCurrencyDisplay != null)
                {
                    GameObject clone = Instantiate(notEnoughCurrencyDisplay, transform.position, Quaternion.identity);
                    clone.GetComponent<Animator>().SetTrigger("Display");
                    clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
                    Destroy(clone, 1f);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            player = other.gameObject;
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            player = null;
            prompt.SetActive(false);
        }
    }

    Color ChooseWeaponNameColor()
    {
        switch (WeaponController.wc.weapons[weaponId].weaponType)
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
