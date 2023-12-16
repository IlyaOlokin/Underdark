using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "DebuffInfos/Harm", fileName = "New HarmInfo")]
public class HarmInfo : DebuffInfo
{
    public HarmType HarmType;
    public int Damage;
    public int ManaDrainAmount;
    public float DmgDelay;
    public float Duration;
    [SerializeField] private GameObject visualPrefab;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetHarmOverTime(this, unitCaster, visualPrefab, effectIcon);
    }
    
    public override string ToString()
    {
        return HarmType switch
        {
            HarmType.Poison =>
                $"With a {chance * 100}% chance inflicts poison on the target, taking away {Damage} HP and {ManaDrainAmount} MP per second for {Duration} seconds.",
            HarmType.Bleed =>
                $"With a {chance * 100}% chance inflicts bleed on the target, taking away {Damage} HP per second for {Duration} seconds.",
            HarmType.ManaDrain =>
                $"With a {chance * 100}% chance inflicts mana drain on the target, taking away {Damage} MP per second for {Duration} seconds.",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
