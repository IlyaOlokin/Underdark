public class InventorySlot : IInventorySlot
{
    public Item Item { get; private set; }

    public string ItemID => IsEmpty ? "" : Item.ID;
    public int Amount { get; set; }
    
    public bool IsFull => !IsEmpty && Amount == Item.StackCapacity;
    public bool IsEmpty => Item == null;
    public bool IsValid { get; set; } = true;
    public ItemType SlotType { get; set; }

    public void SetItem(Item item, int amount = 1)
    {
        Item = item;
        if (item != null) Amount = amount;
    }

    public void Clear()
    {
        if (IsEmpty) return;
        Amount = 0;
        Item = null;
    }
}
