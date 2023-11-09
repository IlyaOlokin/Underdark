using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Potion
{
    [SerializeField] private int healAmount;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        caster.RestoreHP(healAmount, true);
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        res[0] = string.Format(description, healAmount);
        return res;
    }
}
