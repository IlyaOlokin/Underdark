using System;

public interface IDamageable
{
    int MaxHP { get; }
    int CurrentHP { get; }
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    
    void TakeDamage(Unit sender, float damage, bool evadable = false);
    void GetPoisoned(PoisonInfo poisonInfo);
}