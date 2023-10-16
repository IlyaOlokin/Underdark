using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuratePuncture : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = Mathf.Min(caster.Stats.Dexterity * damageStatMultiplier, maxDamage);
        
        OverrideWeaponStats(caster.Weapon);
        var target = FindClosestTarget(caster);
        
        if (target == null) return;
        target.GetComponent<IDamageable>().TakeDamage(damage);
        
        // Instantiate visual
    }
}
