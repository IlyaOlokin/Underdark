using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : IInventorySlot
{
    public Item Item { get; private set; }
    public Type ItemType => typeof(Item);
    public int Amount { get; set; }

    //public int Amount => IsEmpty ? 0 : Item.Amount;
    //public int Capacity { get; private set; }
    
    public bool IsFull => !IsEmpty && Amount == Item.StackCapacity;
    public bool IsEmpty => Item == null;
    
    public void SetItem(Item item, int amount)
    {
        if (!IsEmpty) return;
        Item = item;
        Amount = amount;
        //Capacity = Item.StackCapacity;
    }

    public void Clear()
    {
        if (IsEmpty) return;
        Amount = 0;
        Item = null;
    }
}
