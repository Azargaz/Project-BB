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
    public static bool _playerHasChosenWeapon;
    bool playerHasChosenWeapon;
    bool minibossSpawned = false;
    public Monster[] minibosses;
    GameObject miniboss;

    [System.Serializable]
    public class Monster
    {
        public string name;
        public GameObject[] objects;
        public int chanceToSpawnWeight;
    }

    public bool saveThisPedestal;

    void Deactivate()
    {
        if(_playerHasChosenWeapon && !saveThisPedestal)
        {
            if (prompt.activeInHierarchy)
                prompt.SetActive(false);

            transform.FindChild("Weapon").gameObject.SetActive(false);
        }        
    }

    int RollWithWeights(Monster[] array)
    {
        int summedWeights = 0;
        int returnInt = 0;

        if (array.Length <= 1)
            return 0;

        for (int x = 0; x < array.Length; x++)
        {
            summedWeights += array[x].chanceToSpawnWeight;
        }

        for (int x = 0; x < array.Length; x++)
        {
            int random = Random.Range(0, summedWeights);
            random -= array[x].chanceToSpawnWeight;

            if (random <= 0)
            {
                returnInt = x;
                break;
            }
        }

        return returnInt;
    }

    void Start ()
    {
        weaponId = Random.Range(1, WeaponManager.wp.weapons.Length);
	}

    void Update()
    {
        if(playerHasChosenWeapon != _playerHasChosenWeapon)
        {
            Deactivate();
            playerHasChosenWeapon = _playerHasChosenWeapon;
        }

        if (_playerHasChosenWeapon && !saveThisPedestal)
        {            
            return;
        }

        if (!minibossSpawned && saveThisPedestal)
        {
            minibossSpawned = true;
            int rollMiniboss = RollWithWeights(minibosses);
            GameObject minibossToSpawn = minibosses[rollMiniboss].objects[Random.Range(0, minibosses[rollMiniboss].objects.Length)];
            miniboss = Instantiate(minibossToSpawn, transform.position, Quaternion.identity);

            Room thisRoom = GetComponentInParent<Room>();
            thisRoom.BlockRoom();
        }
        else if(minibossSpawned)
        {
            if(miniboss == null)
            {
                Room thisRoom = GetComponentInParent<Room>();
                thisRoom.UnblockRoom();
            }
        }

        if (weaponSprite != null && WeaponManager.wp.weapons[weaponId] != null)
        {
            weaponSprite.sprite = WeaponManager.wp.weapons[weaponId].sprite;
            promptText.text = (_playerHasChosenWeapon ? "" : "Choose one weapon and defeat powerful monster to escape\n") + WeaponManager.wp.weapons[weaponId].name + (_playerHasChosenWeapon ? "\nPress Q to pickup" : "\nPress Q to pickup");
        }

        if(player != null)
        {
            if(Input.GetButtonDown("PickupWeapon") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
            {
                SwapWeapon();
                saveThisPedestal = true;
                _playerHasChosenWeapon = true;
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
        if(other.gameObject.layer == 8 && !(_playerHasChosenWeapon && !saveThisPedestal))
        {
            player = other.gameObject;
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8 && !(_playerHasChosenWeapon && !saveThisPedestal))
        {
            player = null;
            prompt.SetActive(false);
        }
    }
}
