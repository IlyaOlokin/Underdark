using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour, IInventoryUI
{
    public Player Player { get; private set; }
    public Inventory Inventory { get; private set; }
    private UIInventorySlot selectedSlot;

    [Header("Slots")]
    [SerializeField] private UIInventorySlot[] slots;
    
    [SerializeField] private UIInventorySlot head;
    [SerializeField] private UIInventorySlot body;
    [SerializeField] private UIInventorySlot legs;
    [SerializeField] private UIInventorySlot weapon;
    [SerializeField] private UIInventorySlot shield;
    [SerializeField] private UIInventorySlot[] accessories;
    
    [SerializeField] private UIInventorySlot[] executableSlots;

    [Header("Money Display")] 
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Stats Text")] 
    [SerializeField] private TextMeshProUGUI attackText; 
    [SerializeField] private TextMeshProUGUI armorText;

    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;
    
    public void Init(Player player)
    {
        Player = player;
    }

    public void AwakeInit()
    {
        Inventory = Player.Inventory;
        
        var inventorySlots = Inventory.GetAllSlots();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            slots[i].SetSlot(inventorySlots[i], this);
        }

        SetEquipmentSlots();
        SetExecutableSlots();
    }

    private void SetExecutableSlots()
    {
        for (int i = 0; i < Inventory.ExecutableSlots.Count; i++)
        {
            executableSlots[i].SetSlot(Inventory.ExecutableSlots[i], this);
        }
    }

    private void SetEquipmentSlots()
    {
        var equipmentSlots = Inventory.Equipment;
        head.SetSlot(equipmentSlots.Head, this);
        body.SetSlot(equipmentSlots.Body, this);
        legs.SetSlot(equipmentSlots.Legs, this);
        weapon.SetSlot(equipmentSlots.Weapon, this);
        shield.SetSlot(equipmentSlots.Shield, this);
        
        for (var i = 0; i < equipmentSlots.Accessories.Count; i++)
            accessories[i].SetSlot(equipmentSlots.Accessories[i], this);
    }

    private void OnEnable()
    {
        Inventory.CheckEquipmentFit();
        
        Inventory.OnInventoryChanged += UpdateUI;
        Player.Money.OnMoneyChanged += UpdateMoneyDisplay;
        UpdateUI();
        UpdateMoneyDisplay();
    }

    private void UpdateUI()
    {
        foreach (var slot in slots)
        {
            slot.Refresh();
        }
        
        head.Refresh();
        body.Refresh();
        legs.Refresh();
        weapon.Refresh();
        shield.Refresh();

        foreach (var accessory in accessories)
        {
            accessory.Refresh();
        }

        foreach (var executableSlot in executableSlots)
        {
            executableSlot.Refresh();
        }
        
        UpdateSelectedSlot();

        armorText.text = Player.GetTotalArmor().ToString();
        attackText.text = Player.GetWeapon().Damage.ToString(Player.Stats.GetTotalStatValue(BaseStat.Strength),
            Player.Params.GetDamageAmplification(Player.GetWeapon().Damage.DamageType));

        var secondaryWeapon = Player.Inventory.Equipment.GetSecondaryWeapon();
        if (secondaryWeapon != null)
        {
            var text = secondaryWeapon.Damage.ToString(Player.Stats.GetTotalStatValue(BaseStat.Strength),
                Player.Params.GetDamageAmplification(secondaryWeapon.Damage.DamageType));
            attackText.text += $"\n{text}";
        }
    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = Player.Money.GetMoneyString();
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= UpdateUI;
        Player.Money.OnMoneyChanged -= UpdateMoneyDisplay;
        
        FormatInventory();
    }

    private void FormatInventory()
    {
        Inventory.Format();
    }
    
    public void SelectSlot(UIInventorySlot selectedSlot)
    {
        if (this.selectedSlot != null)
            this.selectedSlot.OnDeselect();
        
        this.selectedSlot = selectedSlot;
        this.selectedSlot.OnSelect();

        UpdateSelectedSlot();
    }

    public void UpdateSelectedSlot()
    {
        if (selectedSlot == null || selectedSlot.Slot.IsEmpty)
            itemDescription.ResetDescriptionActive(false);
        else
            itemDescription.ShowItemDescription(selectedSlot.Slot.Item, Player, selectedSlot.Slot);
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            //slot.Clear();
        }
    }
}
