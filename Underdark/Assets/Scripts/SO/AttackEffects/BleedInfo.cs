using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Bleed", fileName = "New BleedInfo")]
public class BleedInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    [SerializeField] private GameObject visualPrefab;
    
    public override void Execute(IAttacker caster, Unit receiver, Unit unitCaster)
    {
        receiver.GetBleed(this, unitCaster, visualPrefab);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts bleed on the enemy, dealing {Damage} damage per second for {Duration} seconds.";
    }
}
