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
    List<int> usedWeaponIDs = new List<int>();
    List<int> usedWeaponIDs2 = new List<int>();
    bool switchUsedWeapons = false;
    public WeaponDrops[] weaponDropChances;
    public bool initializeWeaponPedestals;
    public static WeaponPedestalController WPC;

    [System.Serializable]
    public class WeaponDrops
    {
        public string name;
        public int chanceToSpawnWeight;
    }

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

    public void RerollWeapons()
    {
        Reroll();
    }

    void Reroll()
    {
        switchUsedWeapons = !switchUsedWeapons;

        if(switchUsedWeapons)
        {
            usedWeaponIDs.Add(WeaponController.wc.currentWeapon);
            usedWeaponIDs2.Clear();
        }
        else
        {
            usedWeaponIDs2.Add(WeaponController.wc.currentWeapon);
            usedWeaponIDs.Clear();
        }

        for (int i = 0; i < weaponPedestals.Count; i++)
        {
            if (i == weaponPedestals.Count - 1)
            {
                weaponPedestals[i].rerollPedestal = true;
                break;
            }

            int ID;

            if(switchUsedWeapons)
            {
                do
                {
                    ID = RollWithWeights(weaponDropChances);
                }
                while (usedWeaponIDs.Contains(ID));
            }
            else
            {
                do
                {
                    ID = RollWithWeights(weaponDropChances);
                }
                while (usedWeaponIDs2.Contains(ID));
            }

            usedWeaponIDs.Add(ID);
            usedWeaponIDs2.Add(ID);

            weaponPedestals[i].weaponId = ID;                
        }
    }

    void Initialize()
    {
        usedWeaponIDs.Clear();
        usedWeaponIDs.Add(WeaponController.wc.currentWeapon);

        for (int i = 0; i < weaponPedestals.Count; i++)
        {
            if (i == weaponPedestals.Count - 1)
            {
                weaponPedestals[i].rerollPedestal = true;
                break;
            }

            int ID = 0;

            do
            {
                ID = RollWithWeights(weaponDropChances);
            }
            while (usedWeaponIDs.Contains(ID));

            weaponPedestals[i].weaponId = ID;
            usedWeaponIDs.Add(ID);
        }
    }

    void Update()
    {
        if(weaponPedestals.Count > 0 && !initializeWeaponPedestals)
        {
            initializeWeaponPedestals = true;
            Initialize();
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
        switchUsedWeapons = false;
        minibossSpawned = false;
        lastWeaponPedestal = null;
        weaponPedestals.Clear();
        usedWeaponIDs.Clear();
    }

    int RollWithWeights(WeaponDrops[] array)
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
