using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    [SerializeField]
    int currentCurrency;

    public static CurrencyController CC;

    void Awake()
    {
        CC = this;
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
}
