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

    public Damage(Damage damage, int str)
    {
        minDamage = damage.minDamage + str;
        maxDamage = damage.maxDamage + str;
    }
    
    public int GetValue()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }

    public string ToString()
    {
        if (minDamage == maxDamage) return minDamage.ToString();
        return $"{minDamage}-{maxDamage}";
    }
}
