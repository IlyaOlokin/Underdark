using System;
using System.Collections.Generic;

public interface IInventory
{
    int Capacity { get; set; }
    bool IsFull { get; }
    
    int GetItemAmount(Type itemType);

    bool TryAddItem(Item item, int amount);
    void Remove(Type itemType, int amount = 1);
    bool HasItem(Type type, out Item item);

    Item GetItem(Type itemType);

    Item[] GetAllItems();
    Item[] GetAllItems(Type itemType);
    Item[] GetEquippedItems();

    
}
