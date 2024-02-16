using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Slow", fileName = "New SlowInfo")]

public class SlowInfo : DebuffInfo
{
    public float SlowAmount;
    public float Duration;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetSlowed(this, effectIcon);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance slows the target by {SlowAmount * 100}% for {Duration} seconds.";
    }
}
