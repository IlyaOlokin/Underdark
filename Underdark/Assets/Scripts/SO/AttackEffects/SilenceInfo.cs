using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Silence", fileName = "New SilenceInfo")]

public class SilenceInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetSilence(this, effectIcon);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts silence on the target.";
    }
}
