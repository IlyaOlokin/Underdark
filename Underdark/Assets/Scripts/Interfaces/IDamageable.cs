using System;
using UnityEngine;

public interface IDamageable
{
    Transform Transform { get; }

    int MaxHP { get; }
    int CurrentHP { get; }
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    
    bool TakeDamage(Unit sender, IAttacker attacker, DamageInfo damageInfo, bool evadable = true, float armorPierce = 0f);
    
}