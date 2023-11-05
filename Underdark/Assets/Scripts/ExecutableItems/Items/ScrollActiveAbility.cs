using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollActiveAbility : ExecutableItem
{
    [SerializeField] private ActiveAbilityItem item;
    public override void Execute(Unit caster)
    {
        caster.Inventory.TryAddActiveAbilityItem(item);
    }

    public override string[] ToString()
    {
        var res = new string[1];
        res[0] = string.Format(description, item.name , "100%");
        return res;
    }
}
