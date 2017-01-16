using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    [SerializeField]
    int currentCurrency;
    int initialRerollCost;
    public int rerollCost;

    public static CurrencyController CC;

    void Awake()
    {
        CC = this;
        initialRerollCost = rerollCost;
    }

    public int CheckCurrency()
    {
        return currentCurrency;
    }

    public void RemoveCurrency(int amount)
    {
        if (amount < currentCurrency)
            currentCurrency -= amount;
        else
            currentCurrency = 0;
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
    }

    public void ResetCurrency()
    {
        currentCurrency = 0;
    }

    public bool RerollWeapons()
    {
        if (currentCurrency < rerollCost)
            return false;

        currentCurrency -= rerollCost;
        rerollCost *= 2;
        return true;
    }

    public void ResetRerollCost()
    {
        rerollCost = initialRerollCost;
    }
}
