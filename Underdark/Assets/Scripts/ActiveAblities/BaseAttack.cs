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
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        this.caster = caster;
        abilityLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(exp);
        
        base.attackDir = caster.GetAttackDirection(caster.GetWeapon().AttackDistance);
        
        StartCoroutine(ExecuteAttack());
    }

    protected override void InitDamage(Unit caster, float damageMultiplier = 1f)
    {
        int damage = (int) ((currentWeapon.Damage.GetValue() +
                     caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel)) * damageMultiplier);
        damageInfo = new DamageInfo(mustAggro);
        damageInfo.AddDamage(damage, currentWeapon.Damage.DamageType,
            caster.Params.GetDamageAmplification(currentWeapon.Damage.DamageType));
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
        InitDamage(caster);
        Attack();
        StartVisual(caster, reversed);
    }

    public void Attack()
    {
        var hitUnits = FindAllTargets(caster, caster.transform.position, currentWeapon.AttackDistance, currentWeapon.AttackRadius);

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

    private new List<Collider2D> FindAllTargets(Unit caster, Vector3 center, float distance, float attackAngle, List<IDamageable> objectsToIgnore = null)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(center, distance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable)) continue;
            if (objectsToIgnore != null && objectsToIgnore.Contains(damageable)) continue;
            if (!HitCheck(center, collider.transform, contactFilter)) continue;
            
            Vector3 dir = collider.transform.position - center;
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < attackAngle / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }
}
