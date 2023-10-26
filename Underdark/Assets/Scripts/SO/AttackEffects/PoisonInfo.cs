using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "DebuffInfos/Poison", fileName = "New PoisonInfo")]
public class PoisonInfo : DebuffInfo
{
    public float Damage;
    public float DmgDelay;
    public float Duration;
    
    public override void Execute(Unit receiver)
    {
        receiver.GetPoisoned(this);
    }
}
