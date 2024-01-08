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
        return Weapon.IsValid ? (MeleeWeapon) Weapon.Item : null;
    }
    
    public IPassiveHolder GetShieldSlotPassiveHolder()
    {
        return Shield.IsValid ? (IPassiveHolder) Shield.Item : null;
    }

    public Armor GetArmor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Head:
                return Head.IsValid ? Head.Item as Armor : null;
            case ItemType.Body:
                return Body.IsValid ? Body.Item as Armor : null;
            case ItemType.Legs:
                return Legs.IsValid ? Legs.Item as Armor : null;
            case ItemType.Shield:
                return Shield.IsValid ? Shield.Item as Armor : null;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }

    public List<Accessory> GetAllAccessories()
    {
        List<Accessory> res = new List<Accessory>();

        foreach (var accessorySlot in Accessories)
        {
            if (accessorySlot.IsEmpty || !accessorySlot.IsEmpty) continue;
            res.Add((Accessory)accessorySlot.Item);
        }

        return res;
    }
}
