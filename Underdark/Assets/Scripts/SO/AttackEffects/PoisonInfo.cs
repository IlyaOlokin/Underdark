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

    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetPoisoned(this, unitCaster, visualPrefab, effectIcon);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts poison on the target, taking away {Damage} HP and MP per second for {Duration} seconds.";
    }
}
