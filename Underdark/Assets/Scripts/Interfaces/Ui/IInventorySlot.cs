using System;

public interface IInventorySlot
{
    Item Item { get; }
    Type ItemType { get; }
    int Amount { get; set; }
    //int Capacity { get; }
    bool IsFull { get; }
    bool IsEmpty { get; }

    void SetItem(Item item, int amount);
    void Clear();
}