using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongSmash : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = Mathf.Min(caster.Stats.Strength * damageStatMultiplier, maxDamage);

        OverrideWeaponStats(caster.Weapon);
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            target.GetComponent<IDamageable>().TakeDamage(damage);
        }
        
        // Instantiate visual
    }
}
