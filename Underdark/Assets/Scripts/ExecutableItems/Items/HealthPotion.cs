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
}
