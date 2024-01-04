using System;
using System.Collections.Generic;

[Serializable]
public class Equipment
{
    public IInventorySlot Head = new InventorySlot();
    public IInventorySlot Body = new InventorySlot();
    public IInventorySlot Legs = new InventorySlot();
    public IInventorySlot Weapon = new InventorySlot();
    public IInventorySlot Shield = new InventorySlot();

    public List<IInventorySlot> Accessories = new();

    private const int AccessoriesCount = 2;

    public Equipment()
    {
        for (int j = 0; j < AccessoriesCount; j++)
        {
            Accessories.Add(new InventorySlot());
        }
    }

    public MeleeWeapon GetWeapon()
    {
        return (MeleeWeapon) Weapon.Item;
    }
    
    public IPassiveHolder GetShieldSlotPassiveHolder()
    {
        return (IPassiveHolder) Shield.Item;
    }

    public Armor GetArmor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Head:
                return (Armor) Head.Item;
            case ItemType.Body:
                return (Armor) Body.Item;
            case ItemType.Legs:
                return (Armor) Legs.Item;
            case ItemType.Shield:
                return Shield.Item as Armor;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }

    public List<Accessory> GetAllAccessories()
    {
        List<Accessory> res = new List<Accessory>();

        foreach (var accessorySlot in Accessories)
        {
            if (accessorySlot.IsEmpty) continue;
            res.Add((Accessory)accessorySlot.Item);
        }

        return res;
    }
}
