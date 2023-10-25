using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Stun", fileName = "New StunInfo")]

public class StunInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(Unit damageable)
    {
        damageable.GetStunned(this);
    }
}
