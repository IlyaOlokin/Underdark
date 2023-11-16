using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAbility
{
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        
        var healAmount = (int) Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);
    }
    
    public override string[] ToString()
    {
        var res = new string[3];
        res[0] = description;
        res[1] = $"Heal: {statMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {maxValue})";
        if (ManaCost != 0) res[2] = $"Mana: {ManaCost}";
        return res;
    }
}
