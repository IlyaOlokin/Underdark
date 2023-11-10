using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ScrollActiveAbility : ExecutableItem
{
    [SerializeField] private ActiveAbilitySO item;
    [SerializeField] private BaseStat baseStat;
    [SerializeField] private int param;
    public override void Execute(Unit caster)
    {
        if (Random.Range(0f, 1f) <= CalculateChance(caster))
        {
            caster.Inventory.TryAddActiveAbilityItem(item);
        }
    }

    private float CalculateChance(Unit caster)
    {
        var x = Mathf.Exp(caster.Stats.GetTotalStatValue(baseStat) - param);
        var y = Mathf.Exp(param - caster.Stats.GetTotalStatValue(baseStat));
        return (x - y) / (2 * (x + y)) + 0.5f;
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        res[0] = string.Format(description, item.Name , Mathf.Floor(CalculateChance(owner) * 100));
        return res;
    }
}