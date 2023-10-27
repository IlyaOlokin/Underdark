using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Bleed", fileName = "New BleedInfo")]
public class BleedInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    
    public override void Execute(Unit receiver)
    {
        receiver.GetBleed(this);
    }
}
