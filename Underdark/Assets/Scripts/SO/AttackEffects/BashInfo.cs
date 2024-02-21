using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Bash", fileName = "New BashInfo")]

public class BashInfo : DebuffInfo
{
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (Random.Range(0f, 1f) > chance) return;
        
        receiver.ResetAbilityCoolDowns();
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance bashes the target.";
    }
}
