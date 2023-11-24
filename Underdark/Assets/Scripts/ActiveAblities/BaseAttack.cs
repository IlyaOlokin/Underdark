using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : ActiveAbility, IAttacker
{
    public Transform Transform { get; }

    [Header("Visual")] 
    [SerializeField] private BaseAttackVisual baseAttackVisual;
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        
        Attack();
        
        StartVisual(caster);
    }

    private void StartVisual(Unit caster)
    {
        baseAttackVisual.Swing(caster.GetAttackDirAngle(), AttackAngle, AttackDistance);
    }

    public void Attack()
    {
        var hitUnits = FindAllTargets(caster);
        var addDamage = caster.Stats.GetTotalStatValue(baseStat) * statMultiplier;

        foreach (var collider in hitUnits)
        {
            if (collider.TryGetComponent(out IDamageable unit))
            {
                if (unit.TakeDamage(caster, this, caster.GetTotalDamage().GetValue() + addDamage,
                        armorPierce: caster.GetWeapon().ArmorPierce)) 
                {
                    foreach (var debuffInfo in caster.GetWeapon().DebuffInfos)
                    {
                        debuffInfo.Execute(caster, collider.GetComponent<Unit>(), caster);
                    }
                }
            }
        }
    }

    public void Attack(IDamageable unit)
    {
        throw new System.NotImplementedException();
    }
}
