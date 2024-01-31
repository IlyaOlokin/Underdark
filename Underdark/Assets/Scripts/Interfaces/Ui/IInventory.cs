using System;
using System.Collections.Generic;

public interface IInventory
{
    int Capacity { get; set; }
    bool IsFull { get; }
    
    int GetItemAmount(string itemID);

    int TryAddItem(Item item, int amount);
    bool Remove(IInventorySlot inventorySlot, int amount = 1);
    bool HasItem(string itemID, out Item item);

    Item GetItem(string itemID);

    Item[] GetAllItems();
    Item[] GetAllItems(string itemID);
}
