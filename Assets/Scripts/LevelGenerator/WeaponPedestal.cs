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
        if (weaponPedestalSpriteRenderer != null && WeaponManager.wp.weapons[weaponId] != null && prompt != null && promptText != null)
        {
            weaponPedestalSpriteRenderer.sprite = WeaponManager.wp.weapons[weaponId].sprite;
            promptText.text = (playerHasChosenWeapon ? "" : "Choose one weapon and defeat powerful monster to escape\n") + WeaponManager.wp.weapons[weaponId].name + "\nPress Q to pickup";
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

        WeaponPedestalController.WPC.Activate(this);
        weaponIdStored = player.GetComponentInChildren<WeaponManager>().currentWeapon;
        player.GetComponentInChildren<WeaponManager>().currentWeapon = weaponId;
        weaponId = weaponIdStored;
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
