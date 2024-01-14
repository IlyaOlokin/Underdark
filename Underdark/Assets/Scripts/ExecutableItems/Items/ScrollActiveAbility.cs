using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;

public class ScrollActiveAbility : ExecutableItem
{
    [SerializeField] private ActiveAbilitySO item;
    [SerializeField] private BaseStat baseStat;
    [SerializeField] private int param;
    
    public override bool Execute(Unit caster)
    {
        if (caster.Inventory.HasActiveAbility(item.ID, out Item itemInInventory))
        {
            NotificationManager.Instance.SendNotification(new Notification(itemInInventory.Sprite, "You already know this spell."));
            return false;
        }
        
        if (Random.Range(0f, 1f) <= CalculateChance(caster))
        {
            if (!caster.Inventory.TryAddActiveAbilityItem(item))
            {
                NotificationManager.Instance.SendNotification(new Notification(item.Sprite, "You've run out of space."));
                return false;
            }
        }
        
        return true;
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
    
    public override string[] ToStringAdditional()
    {
        List<string> res = new List<string>();
        
        res.Add($"Learn chance based on {UnitStats.GetStatFullString(baseStat)}.");
        
        return res.ToArray();
    }
}
