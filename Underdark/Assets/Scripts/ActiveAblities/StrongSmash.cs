using UnityEngine;

public class StrongSmash : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.Strength * damageStatMultiplier, maxDamage);

        OverrideWeaponStats(caster.GetWeapon());
        Attack();
        
        // Instantiate visual
    }

    public void Attack()
    {
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            if (target.GetComponent<IDamageable>().TakeDamage(caster, damage))
            {
                foreach (var debuffInfo in debuffInfos)
                {
                    debuffInfo.Execute(caster, target.GetComponent<Unit>());
                }
            }
        }
    }

    public void Attack(IDamageable unit)
    {
        throw new System.NotImplementedException();
    }
}
