using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldAbility : ActiveAbility
{
    
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);

        var shieldHp = (int) Mathf.Max(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel), MaxValue.GetValue(abilityLevel));
        caster.GetEnergyShield(shieldHp, AttackAngle.GetValue(abilityLevel));
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