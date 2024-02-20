using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/MoveSpeedAmplification", fileName = "New Move Speed Amplification")]
public class MoveSpeedAmplificationSO : PassiveSO
{
    public float Value;
    
    public override string ToString()
    {
        return $"Increases move speed by {Value * 100}%.";
    }
}