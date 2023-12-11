using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money
{
    private int count;
    public event Action<int> OnMoneyChanged; 

    public void AddMoney(int amount)
    {
        count += amount;
        OnMoneyChanged?.Invoke(count);
    }

    public void SetMoney(int amount)
    {
        count = amount;
        OnMoneyChanged?.Invoke(count);

    }

    public int GetMoney() => count;
    
    public bool TrySpendMoney(int amount)
    {
        if (amount > count) return false;

        count -= amount;
        OnMoneyChanged?.Invoke(count);
        return true;
    }
}
