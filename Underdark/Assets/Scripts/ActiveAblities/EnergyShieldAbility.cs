using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldAbility : ActiveAbility
{
    [SerializeField] private int shieldHP;
    [SerializeField] private float shieldRadius;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);

        caster.GetEnergyShield(shieldHP, shieldRadius);
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }
    
    public override string[] ToString()
    {
        var res = new string[5];
        res[0] = description;
        res[1] = $"Shield durability: {shieldHP}";
        if (ManaCost != 0)       res[2] = $"Mana: {ManaCost}"; 
        res[3] = $"Shield Radius: {shieldRadius}";
        return res;
    }
}