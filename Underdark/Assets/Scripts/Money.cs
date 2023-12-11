using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money
{
    private int count;

    public void AddMoney(int amount)
    {
        count += amount;
        Debug.Log(count);
    }

    public void SetMoney(int amount)
    {
        count = amount;
    }
    
    public bool TrySpendMoney(int amount)
    {
        if (amount > count) return false;

        count -= amount;
        return true;
    }
}
