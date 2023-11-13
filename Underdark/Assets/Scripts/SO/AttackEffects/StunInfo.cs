using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Stun", fileName = "New StunInfo")]

public class StunInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetStunned(this, effectIcon);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance stuns the enemy for {Duration} seconds.";
    }
    
}
