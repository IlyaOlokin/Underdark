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
            if (target.GetComponent<IDamageable>().TakeDamage(caster, damage))
            {
                foreach (var debuffInfo in debuffInfos)
                {
                    debuffInfo.Execute(target.GetComponent<Unit>());
                }
            }
        }
        
        // Instantiate visual
    }
}
