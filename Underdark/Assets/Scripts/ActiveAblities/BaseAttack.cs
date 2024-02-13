using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseAttack : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    
    private WeaponSO currentWeapon;

    [FormerlySerializedAs("baseAttackVisual")]
    [Header("Visual")] 
    [SerializeField] private BaseAttackVisual baseAttackVisualPref;
    public override void Execute(Unit caster, int exp, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        this.caster = caster;
        abilityLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(exp);
        
        base.attackDir = caster.GetAttackDirection(caster.GetWeapon().AttackDistance);
        
        int damage = caster.GetWeapon().Damage.GetValue() + caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel);
        damageInfo.AddDamage(damage, caster.GetWeapon().Damage.DamageType, caster.Params.GetDamageAmplification(caster.GetWeapon().Damage.DamageType));

        StartCoroutine(ExecuteAttack());
    }

    private void StartVisual(Unit caster, bool reversed)
    {
        var newVisual = Instantiate(baseAttackVisualPref, transform.position, quaternion.identity, transform);
        newVisual.Swing(caster.GetAttackDirAngle(attackDir), currentWeapon.AttackRadius, currentWeapon.AttackDistance, reversed);
    }

    private IEnumerator ExecuteAttack()
    {
        AttackWithWeapon(caster.GetWeapon(), true);
        yield return new WaitForSeconds(0.1f);
        
        if (caster.HasPassiveOfType<AmbidexteritySO>())
        {
            var secondaryWeapon = caster.Inventory.Equipment.GetSecondaryWeapon();
            if (secondaryWeapon != null)
            {
                AttackWithWeapon(secondaryWeapon, false);
            }
        }
    }

    private void AttackWithWeapon(WeaponSO weapon, bool reversed)
    {
        currentWeapon = weapon;
        Attack();
        StartVisual(caster, reversed);
    }

    public void Attack()
    {
        var hitUnits = FindAllTargets(caster);

        foreach (var collider in hitUnits)
        {
            if (collider.TryGetComponent(out IDamageable unit))
            {
                if (unit.TakeDamage(caster, this, damageInfo, armorPierce: currentWeapon.ArmorPierce)) 
                {
                    foreach (var debuffInfo in currentWeapon.DebuffInfos)
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
        Physics2D.OverlapCircle(caster.transform.position, currentWeapon.AttackDistance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            if (!HitCheck(caster.transform,collider.transform, contactFilter)) continue;
            
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < currentWeapon.AttackRadius / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }
}
