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
}
