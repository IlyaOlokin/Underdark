using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "DebuffInfos/Poison", fileName = "New PoisonInfo")]
public class PoisonInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    [SerializeField] private GameObject visualPrefab;

    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetPoisoned(this, unitCaster, visualPrefab);
    }
}
