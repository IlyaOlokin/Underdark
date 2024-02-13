using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAbility
{
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, base.attackDir);

        var healAmount =
            (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
                MaxValue.GetValue(abilityLevel));
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);
    }

    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }
    
    public override string[] ToString(Unit owner)
    {
        var res = new string[6];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));
        
        res[0] = description;
        res[1] = $"Heal: {StatMultiplier.GetValue(currentLevel)} * {UnitStats.GetStatString(baseStat)} (max: {MaxValue.GetValue(currentLevel)})";
        if (GetManaCost(owner.GetExpOfActiveAbility(ID)) != 0) res[2] = $"Mana: {GetManaCost(owner.GetExpOfActiveAbility(ID))}";
        if (Cooldown.GetValue(currentLevel) != 0) res[5] = $"Cooldown: {Cooldown.GetValue(currentLevel)}";
        return res;
    }
}
