using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPedestalController : MonoBehaviour
{
    public int weaponId;
    int weaponIdStored;
    public SpriteRenderer weaponSprite;
    public GameObject player;
    public GameObject prompt;
    public Text promptText;

	void Start ()
    {
        weaponId = Random.Range(1, 3);
	}

    void Update()
    {
        if(weaponSprite != null && WeaponManager.wp.weapons[weaponId] != null)
        {
            weaponSprite.sprite = WeaponManager.wp.weapons[weaponId].sprite;
            promptText.text = WeaponManager.wp.weapons[weaponId].name + "\nPress Q to pickup";
        }

        if(player != null)
        {
            if(Input.GetButtonDown("PickupWeapon") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
            {
                SwapWeapon();
            }
        }
    }

    void SwapWeapon()
    {
        if (player == null)
            return;

        weaponIdStored = player.GetComponentInChildren<WeaponManager>().currentWeapon;
        player.GetComponentInChildren<WeaponManager>().currentWeapon = weaponId;
        weaponId = weaponIdStored;
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
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
