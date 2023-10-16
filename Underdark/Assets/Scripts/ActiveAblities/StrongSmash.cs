using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongSmash : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = 3 * damageStatMultiplier;

        OverrideWeaponStats(caster.Weapon);
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            target.GetComponent<IDamageable>().TakeDamage(damage);
        }
        
        // Instantiate visual
    }
}
