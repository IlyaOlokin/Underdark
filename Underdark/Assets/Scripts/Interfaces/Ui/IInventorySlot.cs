using System;

public interface IInventorySlot
{
    Item Item { get; }
    Type ItemType { get; }
    int Amount { get; }
    //int Capacity { get; }
    bool IsFull { get; }
    bool IsEmpty { get; }

    void SetItem(Item item);
    void Clear();
}