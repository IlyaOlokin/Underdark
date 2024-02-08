using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAbility
{
    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);

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
        var abilityLevel = GetManaCost(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        res[1] = $"Heal: {StatMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {MaxValue})";
        if (GetManaCost(owner.GetExpOfActiveAbility(ID)) != 0) res[2] = $"Mana: {GetManaCost(owner.GetExpOfActiveAbility(ID))}";
        if (Cooldown.GetValue(abilityLevel) != 0) res[5] = $"Cooldown: {Cooldown.GetValue(abilityLevel)}";
        return res;
    }
}
