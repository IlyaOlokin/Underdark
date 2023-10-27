using UnityEngine;

public class AccuratePuncture : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        damage = Mathf.Min(caster.Stats.Dexterity * damageStatMultiplier, maxDamage);
        
        var target = FindClosestTarget(caster);
        
        if (target == null) return;
        target.GetComponent<IDamageable>().TakeDamage(caster, damage);
        
        // Instantiate visual
    }
}
