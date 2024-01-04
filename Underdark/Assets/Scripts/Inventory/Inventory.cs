using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : IInventory
{
    public int Capacity { get; set; }
    public bool IsFull => slots.All(_ => IsFull);
    
    private List<IInventorySlot> slots;
    public Equipment Equipment { get; private set; }
    public List<IInventorySlot> ExecutableSlots { get; private set; }
    private List<IInventorySlot> activeAbilitySlots;
    public List<IInventorySlot> EquippedActiveAbilitySlots { get; private set; }
    public event Action OnInventoryChanged;
    public event Action OnEquipmentChanged;
    public event Action<bool> OnActiveAbilitiesChanged;
    public event Action OnExecutableItemChanged;
    private Unit unit;
    
    public Inventory(int capacity, int activeAbilityCapacity,  Unit unit)
    {
        Capacity = capacity;
        this.unit = unit;

        slots = new List<IInventorySlot>();
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot());
        }

        ExecutableSlots = new List<IInventorySlot>();
        for (int i = 0; i < 2; i++)
        {
            ExecutableSlots.Add(new InventorySlot());
        }
        
        Equipment = new Equipment();
        
        activeAbilitySlots = new List<IInventorySlot>();
        for (int i = 0; i < activeAbilityCapacity; i++)
        {
            activeAbilitySlots.Add(new InventorySlot());
        }

        EquippedActiveAbilitySlots = new List<IInventorySlot>();
        for (int i = 0; i < 4; i++)
        {
            EquippedActiveAbilitySlots.Add(new InventorySlot());
        }
    }

    public void UpdateInventory()
    {
        OnActiveAbilitiesChanged?.Invoke(true);
        OnExecutableItemChanged?.Invoke();
    }
    
    public int GetItemAmount(string itemID)
    {
        throw new NotImplementedException();
    }

    public int TryAddItem(Item item, int amount = 1)
    {
        if (TryAddToExecutableSlot(item, out IInventorySlot slot))
            return TryAddToSlot(slot, item, amount);
            
        var sameItemSlot = slots.Find(slot => !slot.IsEmpty && slot.Item.ID == item.ID && !slot.IsFull);

        if (sameItemSlot != null)
            return TryAddToSlot(sameItemSlot, item, amount);

        var emptySlot = slots.Find(slot => slot.IsEmpty);
        if (emptySlot != null)
            return TryAddToSlot(emptySlot, item, amount);

        return amount;
    }

    private bool TryAddToExecutableSlot(Item item, out IInventorySlot slot)
    {
        if (item is ExecutableItemSO)
        {
            foreach (var executableSlot in ExecutableSlots)
            {
                if (!executableSlot.IsEmpty && !executableSlot.IsFull && executableSlot.ItemID == item.ID)
                {
                    slot = executableSlot;
                    return true;
                }
            }
        }

        slot = null;
        return false;
    }
    
    public bool TryAddActiveAbilityItem(Item item)
    {
        var sameItemSlot = activeAbilitySlots.Find(slot => !slot.IsEmpty && slot.Item.ID == item.ID && !slot.IsFull);
        if (sameItemSlot != null)
        {
            TryAddToSlot(sameItemSlot, item, 1);
            return true;
        }

        var emptySlot = activeAbilitySlots.Find(slot => slot.IsEmpty);
        if (emptySlot != null)
        {
            TryAddToSlot(emptySlot, item, 1);
            return true;
        }

        return false;
    }

    private int TryAddToSlot(IInventorySlot slot, Item item, int itemAmount)
    {
        bool hasEnoughSpace = slot.Amount + itemAmount <= item.StackCapacity;
        var amountToAdd = hasEnoughSpace ? itemAmount : item.StackCapacity - slot.Amount;
        var amountLeft = itemAmount - amountToAdd;
        var clonedItem = item;
        itemAmount = amountToAdd;

        if (slot.IsEmpty)
        {
            slot.SetItem(clonedItem, itemAmount);
        }
        else
        {
            slot.Amount += amountToAdd;
        }
        
        OnInventoryChanged?.Invoke();

        if (amountLeft <= 0) return amountLeft;

        itemAmount = amountLeft;
        return TryAddItem(item, itemAmount);
    }

    public bool Remove(IInventorySlot inventorySlot, int amount = 1)
    {
        if (inventorySlot.Amount < amount) return false;
        inventorySlot.Amount -= amount;
        if (inventorySlot.Amount == 0)
            inventorySlot.Clear();
        
        OnInventoryChanged?.Invoke();
        return true;
    }
    
    public void ClearSlot(IInventorySlot inventorySlot)
    {
        var itemItemType = inventorySlot.Item.ItemType;
        
        inventorySlot.Clear();
        OnInventoryChanged?.Invoke();
        
        if (itemItemType == ItemType.ActiveAbility) OnActiveAbilitiesChanged?.Invoke(false);
    }

    public IInventorySlot[] GetAllSlots(string itemID)
    {
        return slots.FindAll(slot => !slot.IsEmpty & slot.ItemID == itemID).ToArray();
    }
    public IInventorySlot[] GetAllSlots()
    {
        return slots.ToArray();
    }
    
    public IInventorySlot[] GetAllActiveAbilitySlots()
    {
        return activeAbilitySlots.ToArray();
    }
    
    public bool HasItem(string itemID, out Item item)
    {
        item = GetItem(itemID);
        return item != null;
    }
    
    public bool HasActiveAbility(string itemID, out Item item)
    {
        item = GetActiveAbility(itemID);
        return item != null;
    }
    
    public bool TryMoveItem(IInventorySlot sourceSlot, IInventorySlot targetSlot, ItemType sourceSlotType, ItemType targetSlotType)
    {
        if (sourceSlot.IsEmpty) return false;
        if (sourceSlot == targetSlot) return false;

        if (!IsFitting(sourceSlotType, targetSlot.Item) || !IsFitting(targetSlotType, sourceSlot.Item)) return false;
        
        // check shield with two-handed weapon
        if (targetSlotType == ItemType.Shield && Equipment.GetWeapon()?.WeaponHandedType is WeaponHandedType.TwoHanded)
        {
            NotificationManager.Instance.SendNotification(new Notification(null,
                "You can't equip a shield while wielding a two-handed weapon."));
            return false;
        }

        // two-handed weapon requires no shield equipped
        if ((targetSlotType == ItemType.Weapon && (sourceSlot.Item as MeleeWeapon)?.WeaponHandedType is WeaponHandedType.TwoHanded
            || sourceSlotType == ItemType.Weapon && (targetSlot.Item as MeleeWeapon)?.WeaponHandedType is WeaponHandedType.TwoHanded)
            && !Equipment.Shield.IsEmpty)
        {
            NotificationManager.Instance.SendNotification(new Notification(null,
                "You can't equip a two-handed weapon while wielding a shield."));
            return false;
        }
        
        int newTargetAmount = sourceSlot.Amount;
        int newSourceAmount = targetSlot.Amount;
        
        if (targetSlot.ItemID == sourceSlot.ItemID)
        {
            var maxSlotCapacity = targetSlot.Item.StackCapacity;

            newTargetAmount = Mathf.Min(sourceSlot.Amount + targetSlot.Amount, maxSlotCapacity);
            newSourceAmount = sourceSlot.Amount + targetSlot.Amount - maxSlotCapacity;
        }
        
        var tempItem = targetSlot.Item;
        targetSlot.SetItem(sourceSlot.Item, newTargetAmount);
        if (newSourceAmount > 0) sourceSlot.SetItem(tempItem, newSourceAmount);
        else sourceSlot.Clear();
        
        OnInventoryChanged?.Invoke();
        if (sourceSlotType != ItemType.Any || targetSlotType != ItemType.Any) OnEquipmentChanged?.Invoke();
        if (sourceSlotType == ItemType.ActiveAbility || targetSlotType == ItemType.ActiveAbility) OnActiveAbilitiesChanged?.Invoke(false);
        if (sourceSlotType == ItemType.Executable || targetSlotType == ItemType.Executable) OnExecutableItemChanged?.Invoke();
        
        return true;
    }

    private bool IsFitting(ItemType slotType, Item item)
    {
        return slotType == ItemType.Any 
               || item == null 
               || slotType == item.ItemType && unit.Stats.RequirementsMet(item.Requirements)
               || slotType == ItemType.Shield && unit.HasPassiveOfType<AmbidexteritySO>() && item.ItemType == ItemType.Weapon;
    }
    
    public Item GetItem(string itemID)
    {
        return slots.Find(slot => slot.ItemID == itemID).Item;
    }

    private Item GetActiveAbility(string itemID)
    {
        var itemInSlots = activeAbilitySlots.Find(slot => slot.ItemID == itemID)?.Item;
        if (itemInSlots == null)
            itemInSlots = EquippedActiveAbilitySlots.Find(slot => slot.ItemID == itemID)?.Item;
        return itemInSlots;
    }

    public Item[] GetAllItems()
    {
        var allItems = new List<Item>();
        foreach (var slot in slots)
        {
            if (slot.IsEmpty) continue;
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }

    public Item[] GetAllItems(string itemID)
    {
        var allItems = new List<Item>();
        foreach (var slot in slots)
        {
            if (slot.IsEmpty || slot.ItemID != itemID) continue;
            allItems.Add(slot.Item);
        }

        return allItems.ToArray();
    }
    
    public void Format()
    {
        int filledSlotCount = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty) continue;

            if (filledSlotCount < i)
            {
                slots[filledSlotCount].SetItem(slots[i].Item, slots[i].Amount);
                slots[i].Clear();
            }
            
            filledSlotCount++;
        }
    }
    
    public List<IPassiveHolder> GetAllPassiveHolders()
    {
        var res = new List<IPassiveHolder>()
        {
            Equipment.GetArmor(ItemType.Head),
            Equipment.GetArmor(ItemType.Body),
            Equipment.GetArmor(ItemType.Legs),
            Equipment.GetShieldSlotPassiveHolder(),
            Equipment.GetWeapon(),
        };
        res.AddRange(Equipment.GetAllAccessories());

        return res;
    }

    public Item[] GetEquippedItems()
    {
        throw new NotImplementedException();
    }

    public ExecutableItemSO GetExecutableItem(int index)
    {
        if (!ExecutableSlots[index].IsEmpty)
            return (ExecutableItemSO) ExecutableSlots[index].Item;
        return null;
    }
    
    public ActiveAbility GetEquippedActiveAbility(int index)
    {
        if (!EquippedActiveAbilitySlots[index].IsEmpty)
            return ((ActiveAbilitySO)EquippedActiveAbilitySlots[index].Item).ActiveAbility;
        return null;
    }
}
