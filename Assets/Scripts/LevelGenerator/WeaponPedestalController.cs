using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPedestalController : MonoBehaviour
{   
    bool minibossSpawned = false;
    GameObject lastWeaponPedestal;
    public Monster[] minibosses;
    GameObject miniboss;

    [HideInInspector]
    public List<WeaponPedestal> weaponPedestals;
    List<int> weaponIds = new List<int>();
    public bool initializeWeaponPedestals;
    public static WeaponPedestalController WPC;

    [System.Serializable]
    public class Monster
    {
        public string name;
        public GameObject[] objects;
        public int chanceToSpawnWeight;
    }

    void Awake()
    {
        WPC = this;
    }

    public void Activate(WeaponPedestal source)
    {
        for (int i = 0; i < weaponPedestals.Count; i++)
        {
            if(weaponPedestals[i] != source)
            {
                Destroy(weaponPedestals[i].prompt);
                Destroy(weaponPedestals[i].weaponPedestalSpriteRenderer);
                Destroy(weaponPedestals[i]);
                weaponPedestals.RemoveAt(i);
                i--;
            }
            else
            {
                source.playerHasChosenWeapon = true;
                lastWeaponPedestal = source.gameObject;
            }
        }
    }

    void Update()
    {
        if(weaponPedestals.Count > 0 && !initializeWeaponPedestals)
        {
            initializeWeaponPedestals = true;
            weaponIds.Clear();

            for (int i = 0; i < WeaponManager.wp.weapons.Length; i++)
            {
                if (WeaponManager.wp.equippedWeapon.id == i)
                    continue;

                weaponIds.Add(i);
            }

            for (int i = 0; i < weaponPedestals.Count; i++)
            {
                int ID;

                if (weaponIds.Count > 0)
                {
                    ID = Random.Range(0, weaponIds.Count);

                    weaponPedestals[i].weaponId = weaponIds[ID];

                    weaponIds.RemoveAt(ID);
                }
                else
                {
                    weaponPedestals[i].weaponId = Random.Range(1, WeaponManager.wp.weapons.Length);
                }
            }
        }

        if(lastWeaponPedestal != null)
        {
            if (!minibossSpawned)
            {
                minibossSpawned = true;
                int rollMiniboss = RollWithWeights(minibosses);
                GameObject minibossToSpawn = minibosses[rollMiniboss].objects[Random.Range(0, minibosses[rollMiniboss].objects.Length)];
                miniboss = Instantiate(minibossToSpawn, lastWeaponPedestal.transform.position, Quaternion.identity);

                Room thisRoom = lastWeaponPedestal.GetComponentInParent<Room>();
                miniboss.transform.parent = thisRoom.transform;

                thisRoom.BlockRoom();
            }
            else if (minibossSpawned)
            {
                if (miniboss == null)
                {
                    Room thisRoom = lastWeaponPedestal.GetComponentInParent<Room>();
                    thisRoom.UnblockRoom();
                }
            }
        }
    }

    void OnLevelWasLoaded()
    {
        initializeWeaponPedestals = false;
        minibossSpawned = false;
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
}
