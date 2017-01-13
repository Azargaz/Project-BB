using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPedestal : MonoBehaviour
{
    Text promptText;
    public int weaponId;
    int weaponIdStored;
    public bool playerHasChosenWeapon;
    [HideInInspector]
    public bool rerollPedestal;
    public Sprite rerollSprite;

    GameObject player;
    public GameObject prompt;
    public SpriteRenderer weaponPedestalSpriteRenderer;

    void Start ()
    {
        WeaponPedestalController.WPC.weaponPedestals.Add(this);
        promptText = prompt.GetComponentInChildren<Text>();
	}

    void Update ()
    {
        if (!rerollPedestal && weaponPedestalSpriteRenderer != null && WeaponManager.wp.weapons[weaponId] != null && prompt != null && promptText != null)
        {
            weaponPedestalSpriteRenderer.sprite = WeaponManager.wp.weapons[weaponId].sprite;
            promptText.text = (playerHasChosenWeapon ? "" : "Choose one weapon and defeat powerful monster to escape\n") + WeaponManager.wp.weapons[weaponId].Name + "\nPress Q to pickup";
        }
        else if(rerollPedestal && weaponPedestalSpriteRenderer != null && prompt != null && promptText != null)
        {
            if(rerollSprite != null)
                weaponPedestalSpriteRenderer.sprite = rerollSprite;
            promptText.text = "Reroll for three other weapons\nCost: 1SP\nPress Q to reroll.";
        }

        if (player != null)
        {
            if (Input.GetButtonDown("PickupWeapon") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
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
            weaponIdStored = WeaponManager.wp.currentWeapon;
            WeaponManager.wp.currentWeapon = weaponId;
            weaponId = weaponIdStored;
        }
        else
        {
            WeaponPedestalController.WPC.RerollWeapons();
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
}
