using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : ActiveAbility, IAttacker
{
    public Transform Transform => transform;

    [Header("Visual")] 
    [SerializeField] private BaseAttackVisual baseAttackVisual;
    public override void Execute(Unit caster)
    {
        base.Execute(caster);

        int damage = caster.GetWeapon().Damage.GetValue() + caster.Stats.GetTotalStatValue(baseStat) * statMultiplier;
        damageInfo.AddDamage(damage, caster.GetWeapon().Damage.DamageType, caster.Params.GetDamageAmplification(caster.GetWeapon().Damage.DamageType));
        
        Attack();
        
        StartVisual(caster);
    }

    private void StartVisual(Unit caster)
    {
        baseAttackVisual.Swing(caster.GetAttackDirAngle(attackDir), AttackAngle, AttackDistance);
    }

    public void Attack()
    {
        var hitUnits = FindAllTargets(caster);

        foreach (var collider in hitUnits)
        {
            if (collider.TryGetComponent(out IDamageable unit))
            {
                if (unit.TakeDamage(caster, this, damageInfo, armorPierce: caster.GetWeapon().ArmorPierce)) 
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
