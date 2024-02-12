using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    
    private float baseAttackDistance;
    private float baseAttackRadius;

    [Header("Visual")] 
    [SerializeField] private BaseAttackVisual baseAttackVisual;
    public override void Execute(Unit caster, int exp)
    {
        this.caster = caster;
        abilityLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(exp);
        
        baseAttackDistance = caster.GetWeapon().AttackDistance;
        baseAttackRadius = caster.GetWeapon().AttackRadius;
        attackDir = caster.GetAttackDirection(baseAttackDistance);
        
        int damage = caster.GetWeapon().Damage.GetValue() + caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel);
        damageInfo.AddDamage(damage, caster.GetWeapon().Damage.DamageType, caster.Params.GetDamageAmplification(caster.GetWeapon().Damage.DamageType));
        
        Attack();
        
        StartVisual(caster);
    }

    private void StartVisual(Unit caster)
    {
        baseAttackVisual.Swing(caster.GetAttackDirAngle(attackDir), baseAttackRadius, baseAttackDistance);
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

    private new List<Collider2D> FindAllTargets(Unit caster)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, baseAttackDistance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            if (!HitCheck(caster.transform,collider.transform, contactFilter)) continue;
            
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < baseAttackRadius / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }
}
