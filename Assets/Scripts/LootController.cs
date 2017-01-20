using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootController : MonoBehaviour
{
    public Loot[] loots;

    [System.Serializable]
    public class Loot
    {
        public string Name;
        public GameObject loot;
        public enum Rarity { common, uncommon, rare, veryRare, epic, legendary, meme };
        public Rarity rarity;
        public int chanceToSpawnWeight;
    }

    public static LootController LC;

    void Awake()
    {
        LC = this;
    }

    public GameObject RollLoot(Loot.Rarity maxRarity)
    {
        if (loots.Length <= 0)
            return null;

        int rolledLootId = 0;
        int infinityBreak = 0;

        do
        {
            infinityBreak++;

            if (infinityBreak > 100)
            {
                Debug.LogError("Couldn't RollLoot() after 100 tries.");
                break;
            }

            rolledLootId = RollWithWeights(loots);
        }
        while (loots[rolledLootId].rarity > maxRarity);

        return loots[rolledLootId].loot;
    }

    int RollWithWeights(Loot[] array)
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
