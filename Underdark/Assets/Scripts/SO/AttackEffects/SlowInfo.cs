using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Slow", fileName = "New SlowInfo")]

public class SlowInfo : DebuffInfo
{
    public float SlowAmount;
    public float Duration;
    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetSlowed(this, effectIcon);
    }

    public override string ToString()
    {
        throw new System.NotImplementedException();
    }
}
