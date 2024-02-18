using System;
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

        OnInventoryChanged += CheckEquipmentFit;
        unit.Stats.OnStatsChanged += CheckEquipmentFit;
        unit.Stats.OnLevelUp += CheckEquipmentFit;
    }

    public void UpdateInventory(bool reset = false)
    {
        OnActiveAbilitiesChanged?.Invoke(reset);
        OnExecutableItemChanged?.Invoke();
        CheckEquipmentFit();
    }
    
    public int GetItemAmount(string itemID)
    {
        throw new NotImplementedException();
    }

    public int TryAddItem(Item item, int amount = 1)
    {
        if (TryToEquipItem(item, out IInventorySlot slot))
        {
            var result = TryAddToSlot(slot, item, amount);
            OnEquipmentChanged?.Invoke();
            return result;
        }
            
        var sameItemSlot = slots.Find(inventorySlot => !inventorySlot.IsEmpty && inventorySlot.Item.ID == item.ID && !inventorySlot.IsFull);

        if (sameItemSlot != null)
            return TryAddToSlot(sameItemSlot, item, amount);

        var emptySlot = slots.Find(inventorySlot => inventorySlot.IsEmpty);
        if (emptySlot != null)
            return TryAddToSlot(emptySlot, item, amount);

        return amount;
    }

    private bool TryToEquipItem(Item item, out IInventorySlot slot)
    {
        switch (item.ItemType)
        {
            case ItemType.Executable when TryAddToExecutableSlot(item, out IInventorySlot exSlot):
                slot = exSlot;
                return true;
            case ItemType.Head when Equipment.Head.IsEmpty && IsFitting(Equipment.Head.SlotType, item):
                slot = Equipment.Head;
                return true;
            case ItemType.Body when Equipment.Body.IsEmpty && IsFitting(Equipment.Body.SlotType, item):
                slot = Equipment.Body;
                return true;
            case ItemType.Legs when Equipment.Legs.IsEmpty && IsFitting(Equipment.Legs.SlotType, item):
                slot = Equipment.Legs;
                return true;
            case ItemType.Weapon when Equipment.Weapon.IsEmpty && IsFitting(Equipment.Weapon.SlotType, item):
                if (((WeaponSO)item).WeaponHandedType != WeaponHandedType.TwoHanded || !Equipment.Shield.IsEmpty)
                {
                    slot = null;
                    return false;
                }
                slot = Equipment.Weapon;
                return true;
            case ItemType.Shield when Equipment.Shield.IsEmpty && IsFitting(Equipment.Shield.SlotType, item):
                if (!Equipment.Weapon.IsEmpty && Equipment.GetWeapon().WeaponHandedType == WeaponHandedType.TwoHanded)
                {
                    slot = null;
                    return false;
                }
                slot = Equipment.Shield;
                return true;
            case ItemType.Accessory:
                foreach (var accessorySlot in Equipment.Accessories.Where(accessorySlot => accessorySlot.IsEmpty && IsFitting(accessorySlot.SlotType, item)))
                {
                    slot = accessorySlot;
                    return true;
                }
                
                slot = null;
                return false;
            default:
                slot = null;
                return false;
        }
    }
    
    private bool TryAddToExecutableSlot(Item item, out IInventorySlot slot)
    {
        foreach (var executableSlot in ExecutableSlots)
        {
            if (!executableSlot.IsEmpty && !executableSlot.IsFull && executableSlot.ItemID == item.ID)
            {
                slot = executableSlot;
                return true;
            }
        }
        
        foreach (var executableSlot in ExecutableSlots)
        {
            if (executableSlot.IsEmpty)
            {
                slot = executableSlot;
                return true;
            }
        }
        
        slot = null;
        return false;
    }
    
    public bool TryAddActiveAbilityItem(Item item)
    {
        if (TryToEquipActiveAbilityItem(out IInventorySlot activeAbilitySlot))
        {
            TryAddToSlot(activeAbilitySlot, item, 1);
            OnActiveAbilitiesChanged?.Invoke(false);
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
    
    private bool TryToEquipActiveAbilityItem(out IInventorySlot slot)
    {
        foreach (var activeAbilitySlot in EquippedActiveAbilitySlots)
        {
            if (activeAbilitySlot.IsEmpty)
            {
                slot = activeAbilitySlot;
                return true;
            }
        }
        
        slot = null;
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
        if ((targetSlotType == ItemType.Weapon && (sourceSlot.Item as WeaponSO)?.WeaponHandedType is WeaponHandedType.TwoHanded
            || sourceSlotType == ItemType.Weapon && (targetSlot.Item as WeaponSO)?.WeaponHandedType is WeaponHandedType.TwoHanded)
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
        
        if (sourceSlotType != ItemType.Any || targetSlotType != ItemType.Any) OnEquipmentChanged?.Invoke();
        if (sourceSlotType == ItemType.ActiveAbility || targetSlotType == ItemType.ActiveAbility) OnActiveAbilitiesChanged?.Invoke(false);
        if (sourceSlotType == ItemType.Executable || targetSlotType == ItemType.Executable) OnExecutableItemChanged?.Invoke();
        OnInventoryChanged?.Invoke();
        
        return true;
    }

    private bool IsFitting(ItemType slotType, Item item)
    {
        return slotType == ItemType.Any
               || item == null
               || (slotType == item.ItemType || slotType == ItemType.Shield &&
                   unit.HasPassiveOfType<AmbidexteritySO>() && item.ItemType == ItemType.Weapon && ((WeaponSO) item).WeaponHandedType == WeaponHandedType.OneHanded) &&
               unit.Stats.RequirementsMet(item.Requirements);
    }

    public void CheckEquipmentFit()
    {
        Equipment.Head.IsValid = IsFitting(Equipment.Head.SlotType, Equipment.Head.Item);
        Equipment.Body.IsValid = IsFitting(Equipment.Body.SlotType,  Equipment.Body.Item);
        Equipment.Legs.IsValid = IsFitting(Equipment.Legs.SlotType,  Equipment.Legs.Item);
        Equipment.Weapon.IsValid = IsFitting(Equipment.Weapon.SlotType, Equipment.Weapon.Item);
        Equipment.Shield.IsValid = IsFitting(Equipment.Shield.SlotType, Equipment.Shield.Item);

        foreach (var accessorySlot in Equipment.Accessories)
        {
            accessorySlot.IsValid = IsFitting(accessorySlot.SlotType, accessorySlot.Item);
        }

        foreach (var slot in slots.Where(slot => !slot.IsEmpty))
        {
            slot.IsValid = unit.Stats.RequirementsMet(slot.Item.Requirements);
        }
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
