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
}
