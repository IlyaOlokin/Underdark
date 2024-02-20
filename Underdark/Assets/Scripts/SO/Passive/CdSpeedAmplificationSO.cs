using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/CdSpeedAmplification", fileName = "New Cd Speed Amplification")]
public class CdSpeedAmplificationSO : PassiveSO
{
    public float Value;
    
    public override string ToString()
    {
        return $"Increases ability cooldown speed by {Value * 100}%.";
    }
}
