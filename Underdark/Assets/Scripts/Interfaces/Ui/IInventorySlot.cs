public interface IInventorySlot
{
    Item Item { get; }
    string ItemID { get; }
    int Amount { get; set; }
    bool IsFull { get; }
    bool IsEmpty { get; }
    bool IsValid{ get; set; }
    ItemType SlotType { get; set; }

    void SetItem(Item item, int amount = 1);
    void Clear();
}