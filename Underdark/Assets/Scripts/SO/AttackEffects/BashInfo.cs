using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Bash", fileName = "New BashInfo")]

public class BashInfo : DebuffInfo
{
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetBashed(this);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance bashes the target.";
    }
}
