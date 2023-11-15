using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Freeze", fileName = "New FreezeInfo")]

public class FreezeInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetFrozen(this, effectIcon);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance freeze the target for {Duration} seconds.";
    }
    
}
