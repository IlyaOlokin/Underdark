using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Equipment
{
    public IInventorySlot Head = new InventorySlot();
    public IInventorySlot Body = new InventorySlot();
    public IInventorySlot Legs = new InventorySlot();
    public IInventorySlot Weapon = new InventorySlot();
    public IInventorySlot Shield = new InventorySlot();

    public MeleeWeapon GetWeapon()
    {
        return (MeleeWeapon) Weapon.Item;
    }

    public Armor GetArmor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Any:
                break;
            case ItemType.Head:
                return (Armor) Head.Item;
            case ItemType.Body:
                return (Armor) Body.Item;
            case ItemType.Legs:
                return (Armor) Legs.Item;
            case ItemType.Weapon:
                break;
            case ItemType.Shield:
                return (Armor)Shield.Item;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }

        return null;
    }
}
