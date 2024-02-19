using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    [SerializeField] private int moneyAmountMin;
    [SerializeField] private int moneyAmountMax;
    [SerializeField] protected UnitNotificationEffect moneyDropEffect;

    [SerializeField] private DroppedItem droppedItemPref;
    [SerializeField] private List<ItemToDrop<ArmorSO>> dropArmor;
    [SerializeField] private List<ItemToDrop<WeaponSO>> dropWeapon;
    [SerializeField] private List<ItemToDrop<AccessorySO>> dropAccessory;
    [SerializeField] private List<ItemToDrop<ExecutableItemSO>> dropExecutable;
    
    public void DropItems(IMoneyHolder moneyHolder = null)
    {
        if (moneyAmountMax != 0 && moneyHolder != null)
        {
            DropMoney(moneyHolder);
        }
        
        var roll = Random.Range(0f, 1f);
        foreach (var itemToDrop in dropArmor)
            if (itemToDrop.ChanceToDropLeft <= roll && roll <= itemToDrop.ChanceToDropRight)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }

        foreach (var itemToDrop in dropWeapon)
            if (itemToDrop.ChanceToDropLeft <= roll && roll <= itemToDrop.ChanceToDropRight)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }

        foreach (var itemToDrop in dropAccessory)
            if (itemToDrop.ChanceToDropLeft <= roll && roll <= itemToDrop.ChanceToDropRight)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }

        foreach (var itemToDrop in dropExecutable)
            if (itemToDrop.ChanceToDropLeft <= roll && roll <= itemToDrop.ChanceToDropRight)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }
    }

    public void DropItem(Item item, int amount, Vector2 pos, int force = 4, bool droppedByPlayer = false)
    {
        var newDrop = Instantiate(droppedItemPref, pos, Quaternion.identity);
        newDrop.SetDroppedItem(item, amount, force, droppedByPlayer);
    }

    private void DropMoney(IMoneyHolder moneyHolder)
    {
        var money = Random.Range(moneyAmountMin, moneyAmountMax + 1);
        moneyHolder.Money.AddMoney(money);
        var newEffect = Instantiate(moneyDropEffect, transform.position, Quaternion.identity);
        newEffect.WriteMessage($"+ • {money}");
    }
}
