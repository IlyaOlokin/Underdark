using UnityEngine;

public class AccuratePuncture : ActiveAblity, IAttacker
{
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.Dexterity * damageStatMultiplier, maxDamage);
        
        var target = FindClosestTarget(caster);
        
        if (target == null) return;
        Attack(target.GetComponent<IDamageable>());
        
        // Instantiate visual
    }

    
    public void Attack()
    {
        
    }

    public void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, damage))
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
}
