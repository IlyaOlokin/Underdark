using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScrollActiveAbility : ExecutableItem
{
    [SerializeField] private ActiveAbilitySO item;
    public override void Execute(Unit caster)
    {
        caster.Inventory.TryAddActiveAbilityItem(item);
    }

    public override string[] ToString()
    {
        var res = new string[1];
        res[0] = string.Format(description, item.Name , "100%");
        return res;
    }
}
