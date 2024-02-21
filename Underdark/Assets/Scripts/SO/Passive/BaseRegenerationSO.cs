using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/BaseRegeneration", fileName = "New Base Regeneration")]
public class BaseRegenerationSO : PassiveSO
{
    public RegenType RegenType;
    public float Value;
    
    public override string ToString()
    {
        return RegenType switch
        {
            RegenType.HP => $"Sets HP regeneration to {Value * 100}%.",
            RegenType.MP => $"Sets MP regeneration to {Value * 100}%.",
            RegenType.Both => $"Sets HP and MP regeneration to {Value * 100}%.",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
