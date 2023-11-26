using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/EvasionAmplification", fileName = "New Evasion Amplification")]
public class EvasionAmplificationSO : PassiveSO
{
    public float EvasionChance;

    public override string ToString()
    {
        return $"{EvasionChance * 100}% chance to evade damage.";
    }
}
