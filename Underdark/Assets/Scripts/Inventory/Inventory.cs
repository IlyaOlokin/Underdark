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
    public event Action<Item, int> OnInventoryItemAdded;
    public event Action OnInventoryChanged;
    
    public Inventory(int capacity)
    {
        Capacity = capacity;

        slots = new List<IInventorySlot>();
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot());
        }
    }
    
    public int GetItemAmount(Type itemType)
    {
        throw new NotImplementedException();
    }

    public bool TryAddItem(Item item, int amount)
    {
        var sameItemSlot = slots.Find(slot => !slot.IsEmpty && slot.Item.ID == item.ID && !slot.IsFull);

        if (sameItemSlot != null)
            return TryAddToSlot(sameItemSlot, item, amount);

        var emptySlot = slots.Find(slot => slot.IsEmpty);
        if (emptySlot != null)
            return TryAddToSlot(emptySlot, item, amount);

        return false;
    }

    private bool TryAddToSlot(IInventorySlot slot, Item item, int itemAmount)
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

        if (amountLeft <= 0) return true;

        itemAmount = amountLeft;
        return TryAddItem(item, itemAmount);
    }

    public void Remove(Type itemType, int amount = 1)
    {
        throw new NotImplementedException();
    }

    public IInventorySlot[] GetAllSlots(Type itemType)
    {
        return slots.FindAll(slot => !slot.IsEmpty & slot.ItemType == itemType).ToArray();
    }
    public IInventorySlot[] GetAllSlots()
    {
        return slots.ToArray();
    }

    public bool HasItem(Type type, out Item item)
    {
        item = GetItem(type);
        return item != null;
    }

    public void MoveItem(IInventorySlot fromSlot, IInventorySlot toSlot)
    {
        if (fromSlot.IsEmpty) return;
        if (toSlot.IsFull) return;
        if (!toSlot.IsEmpty && fromSlot.ItemType != toSlot.ItemType) return;

        var slotCapacity = fromSlot.Item.StackCapacity;
        var fits = fromSlot.Amount + toSlot.Amount <= slotCapacity;
        var amountToAdd = fits ? fromSlot.Amount : slotCapacity - toSlot.Amount;
        var amountLeft = fromSlot.Amount - amountToAdd;

        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.Item, fromSlot.Amount);
            fromSlot.Clear();
            OnInventoryChanged?.Invoke();

            //toSlot.Item.Amount += amountToAdd;
            if (fits)
                fromSlot.Clear();
            else
                fromSlot.Amount = amountLeft;
            OnInventoryChanged?.Invoke();
        }
    }
    
    public Item GetItem(Type itemType)
    {
        return slots.Find(slot => slot.ItemType == itemType).Item;
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

    public Item[] GetAllItems(Type itemType)
    {
        var allItems = new List<Item>();
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.ItemType != itemType) continue;
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public Item[] GetEquippedItems()
    {
        throw new NotImplementedException();
    }

    
}
