using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAbility
{
    public override void Execute(Unit caster)
    {
        var healAmount = (int) Mathf.Min(caster.Stats.GetStatValue(baseStat) * statMultiplier, maxValue);
        caster.RestoreHP(healAmount, true);
    }
    
    public override string[] ToString()
    {
        var res = new string[3];
        res[0] = description;
        res[1] = $"Heal: {statMultiplier} * {GetStatString(baseStat)} (max: {maxValue})";
        return res;
    }
}
