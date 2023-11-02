using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalRestoration : ActiveAblity
{
    public override void Execute(Unit caster)
    {
        var healAmount = (int) Mathf.Min(caster.Stats.Intelligence * statMultiplier, maxValue);
        caster.RestoreHP(healAmount, true);
    }
}
