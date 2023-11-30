using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/ManaDrain", fileName = "New ManaDrainInfo")]
public class ManaDrainInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    [SerializeField] private GameObject visualPrefab;

    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetManaDrain(this, unitCaster, visualPrefab, effectIcon);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts mana drain on the target, taking away {Damage} MP per second for {Duration} seconds.";
    }
}