using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : IInventory
{
    public int Capacity { get; set; }
    public bool IsFull => slots.All(_ => IsFull);
    
    private List<IInventorySlot> slots;
    public Equipment Equipment { get; private set; }
    public event Action<Item, int> OnInventoryItemAdded;
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;
    private Unit unit;
    
    public Inventory(int capacity, Unit unit)
    {
        Capacity = capacity;
        this.unit = unit;

        slots = new List<IInventorySlot>();
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot());
        }

        Equipment = new Equipment();
    }
    
    public int GetItemAmount(string itemID)
    {
        throw new NotImplementedException();
    }

    public int TryAddItem(Item item, int amount)
    {
        var sameItemSlot = slots.Find(slot => !slot.IsEmpty && slot.Item.ID == item.ID && !slot.IsFull);

        if (sameItemSlot != null)
            return TryAddToSlot(sameItemSlot, item, amount);

        var emptySlot = slots.Find(slot => slot.IsEmpty);
        if (emptySlot != null)
            return TryAddToSlot(emptySlot, item, amount);

        return amount;
    }

    private int TryAddToSlot(IInventorySlot slot, Item item, int itemAmount)
    {
        bool hasEnoughSpace = slot.Amount + itemAmount <= item.StackCapacity;
        var amountToAdd = hasEnoughSpace ? itemAmount : item.StackCapacity - slot.Amount;
        var amountLeft = itemAmount - amountToAdd;
        var clonedItem = item;
        itemAmount = amountToAdd;

        if (slot.IsEmpty)
        {
            slot.SetItem(clonedItem, itemAmount);
        }
        else
        {
            slot.Amount += amountToAdd;
        }
        
        OnInventoryItemAdded?.Invoke(item, amountToAdd);

        if (amountLeft <= 0) return amountLeft;

        itemAmount = amountLeft;
        return TryAddItem(item, itemAmount);
    }

    public void Remove(string itemID, int amount = 1)
    {
        throw new NotImplementedException();
    }

    public IInventorySlot[] GetAllSlots(string itemID)
    {
        return slots.FindAll(slot => !slot.IsEmpty & slot.ItemID == itemID).ToArray();
    }
    public IInventorySlot[] GetAllSlots()
    {
        return slots.ToArray();
    }
    
    public bool HasItem(string itemID, out Item item)
    {
        item = GetItem(itemID);
        return item != null;
    }

    public void MoveItem(IInventorySlot fromSlot, IInventorySlot toSlot,ItemType fromSlotItemType = ItemType.Any, ItemType toSlotItemType = ItemType.Any)
    {
        if (fromSlot.IsEmpty) return;
        
        // check requirements
        bool equipmentChanged = false;
        if (fromSlot.Item.ItemType != ItemType.Any && toSlotItemType != ItemType.Any) {
            if (!unit.Stats.RequirementsMet(fromSlot.Item.Requirements)) return;
            equipmentChanged = true;
        } 
        // check requirements for swap case
        if (!toSlot.IsEmpty && toSlot.Item.ItemType != ItemType.Any && fromSlotItemType != ItemType.Any) {
            if (!unit.Stats.RequirementsMet(toSlot.Item.Requirements)) return;
            equipmentChanged = true;
        } 

        if (!toSlot.IsEmpty && fromSlot.ItemID != toSlot.ItemID)
        {
            var tempItem = fromSlot.Item;
            var tempAmount = fromSlot.Amount;
            fromSlot.Clear();
            fromSlot.SetItem(toSlot.Item, toSlot.Amount);
            toSlot.Clear();
            toSlot.SetItem(tempItem,tempAmount);
            
            return;
        }
        if (toSlot.IsFull) return;
        
        var slotCapacity = fromSlot.Item.StackCapacity;
        var fits = fromSlot.Amount + toSlot.Amount <= slotCapacity;
        var amountToAdd = fits ? fromSlot.Amount : slotCapacity - toSlot.Amount;
        var amountLeft = fromSlot.Amount - amountToAdd;

        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.Item, fromSlot.Amount);
            fromSlot.Clear();
        }
        else
        {
            toSlot.Amount += amountToAdd;
            if (fits)
                fromSlot.Clear();
            else
                fromSlot.Amount = amountLeft;
        }
        
        OnInventoryChanged?.Invoke();
        if (equipmentChanged) OnEquipmentChanged?.Invoke();
    }
    
    public Item GetItem(string itemID)
    {
        return slots.Find(slot => slot.ItemID == itemID).Item;
    }

    public Item[] GetAllItems()
    {
        var allItems = new List<Item>();
        foreach (var slot in slots)
        {
            if (slot.IsEmpty) continue;
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public Item[] GetAllItems(string itemID)
    {
        var allItems = new List<Item>();
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.ItemID != itemID) continue;
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public Item[] GetEquippedItems()
    {
        throw new NotImplementedException();
    }

    
}
