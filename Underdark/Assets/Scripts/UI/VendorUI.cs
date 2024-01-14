using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class VendorUI : InGameUiWindow, IInventoryUI
{
    private UIInventorySlot selectedSlot;
    
    private bool isPlayersSlot;
    
    public Inventory Inventory { get; private set; }
    [Header("Player Window")]
    [SerializeField] private UIVendorSlot[] slotsPlayer;
    
    [Header("Vendor Window")]
    private Vendor vendor;
    [SerializeField] private UIVendorSlot[] slotsVendor;
    
    [Header("Money Display")] 
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;
    [SerializeField] private BuySellMenu buySellMenu;

    public void Init(Vendor vendor)
    {
        this.vendor = vendor;
    }
    
    protected override void Awake()
    {
        base.Awake();
        Inventory = player.Inventory;
    }

    private void OnEnable()
    {
        SetVendorSlots();
        UpdateUI();
        UpdateMoneyDisplay();
        
        Inventory.OnInventoryChanged += UpdateUI;
        player.Money.OnMoneyChanged += UpdateMoneyDisplay;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= UpdateUI;
        player.Money.OnMoneyChanged -= UpdateMoneyDisplay;

        DeselectSlot();
    }

    private void UpdateUI()
    {
        SetVendorSlots();
        SetPlayerSlots();
        foreach (var slot in slotsPlayer)
        {
            slot.InventorySlot.Refresh();
        }
        
        UpdateSelectedSlot();
    }
    
    private void SetVendorSlots()
    {
        var vendorInventorySlots = vendor.Slots;
        for (int i = 0; i < slotsVendor.Length; i++)
        {
            slotsVendor[i].CostText.gameObject.SetActive(false);

            if (i >= vendorInventorySlots.Count) continue;

            slotsVendor[i].CostText.gameObject.SetActive(true);
            slotsVendor[i].InventorySlot.SetSlot(vendorInventorySlots[i]);

            var color = vendorInventorySlots[i].Item.Cost < player.Money.GetMoney()
                ? "<color=#FFFFFF>"
                : "<color=#FF4E4E>";
            slotsVendor[i].CostText.text = $"{color}• {vendorInventorySlots[i].Item.Cost}";
        }
    }

    private void SetPlayerSlots()
    {
        var playerInventorySlots = Inventory.GetAllSlots();
        var filledSlotIndex = 0;
        for (int i = 0; i < playerInventorySlots.Length; i++)
        {
            slotsPlayer[i].CostText.gameObject.SetActive(false);
            slotsPlayer[filledSlotIndex].InventorySlot.SetSlot(null);

            if (playerInventorySlots[i].IsEmpty) continue;

            slotsPlayer[filledSlotIndex].InventorySlot.SetSlot(playerInventorySlots[i]);
            slotsPlayer[filledSlotIndex].CostText.gameObject.SetActive(true);
            slotsPlayer[filledSlotIndex].CostText.text =
                "• " + slotsPlayer[filledSlotIndex].InventorySlot.Slot.Item.Cost;
            filledSlotIndex++;
        }
    }
    
    private void UpdateMoneyDisplay()
    {
        moneyText.text = player.Money.GetMoneyString();
    }

    public void SelectSlot(UIInventorySlot selectedSlot)
    {
        if (this.selectedSlot != null)
            this.selectedSlot.OnDeselect();
        
        this.selectedSlot = selectedSlot;
        this.selectedSlot.OnSelect();

        isPlayersSlot = slotsPlayer.Select(x => x.InventorySlot).Contains(this.selectedSlot);
        
        UpdateSelectedSlot();
    }

    public void UpdateSelectedSlot()
    {
        if (selectedSlot == null || selectedSlot.Slot == null || selectedSlot.Slot.IsEmpty)
        {
            itemDescription.ResetDescriptionActive(false);
            buySellMenu.ResetButtons();
        }
        else
        {
            itemDescription.ShowItemDescription(selectedSlot.Slot.Item, player, selectedSlot.Slot);
            buySellMenu.UpdateButtons(isPlayersSlot, selectedSlot.Slot);
        }
    }

    public void DeselectSlot()
    {
        if (selectedSlot == null)
        {
            return;
        }
        selectedSlot.OnDeselect();
        selectedSlot = null;
    }
}
