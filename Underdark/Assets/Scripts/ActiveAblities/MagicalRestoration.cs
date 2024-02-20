using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MagicalRestoration : ActiveAbility
{
    [SerializeField] private ScalableProperty<float> healOfMaxHP;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);

        var maxHeal = MaxValue.GetValue(abilityLevel) < 0 ? int.MaxValue : MaxValue.GetValue(abilityLevel);
        var healAmount =
            (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
                maxHeal);

        healAmount += (int) (caster.MaxHP * healOfMaxHP.GetValue(abilityLevel));
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
        if (StatMultiplier.GetValue(currentLevel) != 0)
            res[1] = $"Heal: {StatMultiplier.GetValue(currentLevel)} * {UnitStats.GetStatString(baseStat)} " +
                     HealOfMaxHPToString(currentLevel) + MaxValueToString(currentLevel);
        else
            res[1] = $"Heal: {HealOfMaxHPToString(currentLevel)}";
        if (GetManaCost(owner.GetExpOfActiveAbility(ID)) != 0) res[2] = $"Mana: {GetManaCost(owner.GetExpOfActiveAbility(ID))}";
        if (Cooldown.GetValue(currentLevel) != 0) res[5] = $"Cooldown: {Cooldown.GetValue(currentLevel)}";
        return res;
    }

    private string HealOfMaxHPToString(int level)
    {
        var heal = healOfMaxHP.GetValue(level);
        if (heal <= 0) return "";

        return $"+{heal * 100}% of max HP";
    }
}
