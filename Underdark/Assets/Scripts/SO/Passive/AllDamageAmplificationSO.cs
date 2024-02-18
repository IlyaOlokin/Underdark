using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/ALLDamageAmplification", fileName = "New Damage All Amplification")]

public class AllDamageAmplificationSO : PassiveSO
{
    public float Value;
    
    public override string ToString()
    {
        return $"Increases outgoing damage by {Value * 100}%.";
    }
}
