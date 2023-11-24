using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Damage
{
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    public Damage(Damage damage, int strength)
    {
        minDamage = damage.minDamage + strength;
        maxDamage = damage.maxDamage + strength;
    }
    
    public int GetValue()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }

    public new string ToString(int addDamage = 0)
    {
        if (minDamage == maxDamage) return (minDamage + addDamage).ToString();
        return $"{minDamage + addDamage}-{maxDamage + addDamage}";
    }
}
