using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldAbility : ActiveAbility
{
    private int maxHP;
    private int currentHP;

    private float shieldRadius;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        transform.SetParent(caster.transform);

        var shieldHp = (int) Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel), MaxValue.GetValue(abilityLevel));
        
        maxHP = shieldHp;
        currentHP = maxHP;
        shieldRadius = AttackAngle.GetValue(abilityLevel);
        
        caster.GetEnergyShield(this);
    }
    
    public bool TakeDamage(Unit owner, Unit sender, IAttacker attacker, UnitNotificationEffect newEffect,
        UnitNotificationEffect unitNotificationEffect, ref int newDamage)
    {
        Vector3 dir = attacker == null ? sender.transform.position : attacker.Transform.position -owner.transform.position;
        var angle = Vector2.Angle(dir, owner.GetLastMoveDir());

        var savedDamage = newDamage;

        if (AbsorbDamage(ref newDamage, angle))
        {
            newEffect.WriteDamage(savedDamage, true);

            if (newDamage > 0)
            {
                var newEffectForES = Object.Instantiate(unitNotificationEffect, owner.transform.position, Quaternion.identity);
                newEffectForES.WriteDamage(savedDamage - newDamage, true);
                owner.LooseEnergyShield();
                Destroy(gameObject);
            }
            else
                return true;
        }

        return false;
    }

    private bool AbsorbDamage(ref int damage, float angleToAttacker)
    {
        if (!(angleToAttacker <= shieldRadius / 2f)) return false;

        if (damage > currentHP)
            damage -= currentHP;
        else
        {
            currentHP -= damage;
            damage = 0;
        }
        
        return true;
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }
    
    public override string[] ToString(Unit owner)
    {
        var res = new string[5];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        if (StatMultiplier.GetValue(currentLevel) != 0)
            res[1] =
                $"Shield durability: {StatMultiplier.GetValue(currentLevel)} * {UnitStats.GetStatString(baseStat)}" +
                MaxValueToString(currentLevel);
        if (GetManaCost(owner.GetExpOfActiveAbility(ID)) != 0) res[2] = $"Mana: {GetManaCost(owner.GetExpOfActiveAbility(ID))}"; 
        res[3] = $"Shield Radius: {AttackAngle.GetValue(currentLevel)}";
        return res;
    }
}