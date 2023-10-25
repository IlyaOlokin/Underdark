using UnityEngine;

public class StrongSmash : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = Mathf.Min(caster.Stats.Strength * damageStatMultiplier, maxDamage);

        OverrideWeaponStats(caster.GetWeapon());
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            target.GetComponent<IDamageable>().TakeDamage(caster, damage);
        }
        
        // Instantiate visual
    }
}
