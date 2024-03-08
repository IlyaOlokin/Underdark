using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/InstantKill", fileName = "InstantKill")]

public class InstantKillInfo : DebuffInfo
{
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (Random.Range(0f, 1f) > chance) return;

        receiver.InstantDeath(unitCaster, attacker);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance kills the target.";
    }
}
