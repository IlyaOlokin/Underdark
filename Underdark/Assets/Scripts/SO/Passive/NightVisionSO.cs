using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive/NightVision", fileName = "New Night Vision")]

public class NightVisionSO : PassiveSO
{
    public override string ToString()
    {
        return $"Gives you Night Vision.";
    }
}
