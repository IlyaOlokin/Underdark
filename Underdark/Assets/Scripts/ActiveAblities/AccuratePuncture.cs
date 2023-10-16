using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccuratePuncture : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = 3 * damageStatMultiplier;

        OverrideWeaponStats(caster.Weapon);
        var target = FindClosestTarget(caster);
        
        if (target == null) return;
        target.GetComponent<IDamageable>().TakeDamage(damage);
        
        // Instantiate visual
    }
}
