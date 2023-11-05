using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : ExecutableItem
{
    [SerializeField] private int healAmount;
    
    public override void Execute(Unit caster)
    {
        caster.RestoreHP(healAmount, true);
    }

    public override string[] ToString()
    {
        var res = new string[1];
        res[0] = string.Format(description, healAmount);
        return res;
    }
}
