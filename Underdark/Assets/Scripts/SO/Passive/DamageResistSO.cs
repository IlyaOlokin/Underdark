using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/DamageResist", fileName = "New Damage Resist")]

public class DamageResistSO : PassiveSO
{
    public DamageType DamageType;
    public float Value;
    
    public override string ToString()
    {
        return $"Increases {DamageType} damage resist by {Value * 100}%.";
    }
}
