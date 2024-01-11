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

    public WeaponSO GetWeapon()
    {
        return Weapon.IsValid ? (WeaponSO) Weapon.Item : null;
    }
    
    public IPassiveHolder GetShieldSlotPassiveHolder()
    {
        return Shield.IsValid ? (IPassiveHolder) Shield.Item : null;
    }

    public ArmorSO GetArmor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Head:
                return Head.IsValid ? Head.Item as ArmorSO : null;
            case ItemType.Body:
                return Body.IsValid ? Body.Item as ArmorSO : null;
            case ItemType.Legs:
                return Legs.IsValid ? Legs.Item as ArmorSO : null;
            case ItemType.Shield:
                return Shield.IsValid ? Shield.Item as ArmorSO : null;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }

    public List<AccessorySO> GetAllAccessories()
    {
        List<AccessorySO> res = new List<AccessorySO>();

        foreach (var accessorySlot in Accessories)
        {
            if (accessorySlot.IsEmpty || !accessorySlot.IsEmpty) continue;
            res.Add((AccessorySO)accessorySlot.Item);
        }

        return res;
    }
}
