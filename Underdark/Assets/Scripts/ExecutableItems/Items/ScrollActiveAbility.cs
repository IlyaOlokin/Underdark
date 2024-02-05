using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;

public class ScrollActiveAbility : ExecutableItem
{
    [field:SerializeField] public ActiveAbilitySO Item { get; private set; }
    [SerializeField] private BaseStat baseStat;
    [SerializeField] private int param;
    
    public override bool Execute(Unit caster)
    {
        if (Random.Range(0f, 1f) <= CalculateChance(caster))
        {
            if (caster.Inventory.HasActiveAbility(Item.ID, out Item itemInInventory))
            {
                caster.AddExpToActiveAbility(Item.ID, 1);
                return true;
            }
            
            if (!caster.Inventory.TryAddActiveAbilityItem(Item))
            {
                NotificationManager.Instance.SendNotification(new Notification(Item.Sprite, "You've run out of space."));
                return false;
            }
            caster.AddExpToActiveAbility(Item.ID, 1);
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
        res[0] = string.Format(description, Item.Name , Mathf.Floor(CalculateChance(owner) * 100));
        return res;
    }
    
    public override string[] ToStringAdditional()
    {
        List<string> res = new List<string>();
        
        res.Add($"Learn chance based on {UnitStats.GetStatFullString(baseStat)}.");
        
        return res.ToArray();
    }
}
