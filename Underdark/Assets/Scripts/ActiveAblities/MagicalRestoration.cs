using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAbility
{
    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);
        
        var healAmount = (int) Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);
    }

    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }
    
    public override string[] ToString()
    {
        var res = new string[6];
        res[0] = description;
        res[1] = $"Heal: {statMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {maxValue})";
        if (ManaCost != 0) res[2] = $"Mana: {ManaCost}";
        if (Cooldown != 0) res[5] = $"Cooldown: {Cooldown}";
        return res;
    }
}
