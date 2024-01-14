using System;

public class Money
{
    private int count;
    public event Action OnMoneyChanged; 

    public void AddMoney(int amount)
    {
        count += amount;
        OnMoneyChanged?.Invoke();
    }

    public void SetMoney(int amount)
    {
        count = amount;
        OnMoneyChanged?.Invoke();
    }

    public int GetMoney() => count;
    
    public bool TrySpendMoney(int amount)
    {
        if (amount > count) return false;

        count -= amount;
        OnMoneyChanged?.Invoke();
        return true;
    }

    public string GetMoneyString() => $"• {count}";
}
