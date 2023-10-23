public class InventorySlot : IInventorySlot
{
    public Item Item { get; private set; }

    public string ItemID => Item.ID;
    public int Amount { get; set; }
    
    public bool IsFull => !IsEmpty && Amount == Item.StackCapacity;
    public bool IsEmpty => Item == null;

    public void SetItem(Item item, int amount = 1)
    {
        if (!IsEmpty) return;
        Item = item;
        Amount = amount;
    }

    public void Clear()
    {
        if (IsEmpty) return;
        Amount = 0;
        Item = null;
    }
}
