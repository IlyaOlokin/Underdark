using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonInfo : DebuffInfo
{
    public float damage;
    public float dmgDelay;
    public float duration;
    
    public override void Execute(IDamageable receiver)
    {
        receiver.GetPoisoned(this);
    }
}
