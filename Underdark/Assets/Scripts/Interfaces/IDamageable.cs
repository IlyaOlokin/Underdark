using System;

public interface IDamageable
{
    int MaxHP { get; }
    int CurrentHP { get; }
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    
    bool TakeDamage(Unit sender, float damage, bool evadable = true, float armorPierce = 0f);
    
}