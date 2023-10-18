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

    public int GetValue()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }
}
