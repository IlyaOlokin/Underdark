using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Passive/RegenAmplification", fileName = "New Regen Amplification")]
public class RegenAmplificationSO : PassiveSO
{
    public RegenType RegenType;
    public float Value;
    
    public override string ToString()
    {
        return RegenType switch
        {
            RegenType.HP => $"Increases HP regeneration by {Value * 100}%.",
            RegenType.MP => $"Increases MP regeneration by {Value * 100}%.",
            RegenType.Both => $"Increases HP and MP regeneration by {Value * 100}%.",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}