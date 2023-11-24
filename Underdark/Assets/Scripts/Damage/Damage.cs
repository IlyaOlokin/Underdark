using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Damage
{
    [field:SerializeField] public DamageType DamageType { get; private set; }
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    public Damage(int minDamage, int maxDamage, DamageType damageType = DamageType.Physic, float multiplier = 1)
    {
        this.minDamage = (int) Mathf.Floor(minDamage * multiplier);
        this.maxDamage = (int) Mathf.Floor(maxDamage * multiplier);
        DamageType = damageType;
    }
    
    public int GetValue()
    {
        if (minDamage == maxDamage) return minDamage;
        return Random.Range( minDamage, maxDamage + 1);
    }

    public string ToString(int addDamage = 0)
    {
        if (minDamage == maxDamage) return (minDamage + addDamage).ToString();
        return $"{minDamage + addDamage}-{maxDamage + addDamage}";
    }
}
