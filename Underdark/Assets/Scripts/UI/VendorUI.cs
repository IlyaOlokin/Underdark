using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VendorUI : InGameUiWindow, IInventoryUI
{
    public Inventory Inventory { get; private set; }
    [Header("Player Window")]
    [SerializeField] private UIVendorSlot[] slotsPlayer;
    
    [Header("Vendor Window")]
    private Vendor vendor;
    [SerializeField] private UIVendorSlot[] slotsVendor;

    
    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;

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
        
        Inventory.OnInventoryChanged += UpdateUI;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= UpdateUI;
    }

    private void UpdateUI()
    {
        SetPlayerSlots();
        foreach (var slot in slotsPlayer)
        {
            slot.InventorySlot.Refresh();
        }
    }
    
    private void SetVendorSlots()
    {
        var vendorInventorySlots = vendor.Slots;
        for (int i = 0; i < slotsVendor.Length; i++)
        {
            slotsVendor[i].CostText.gameObject.SetActive(false);

            if (i >= vendorInventorySlots.Count) continue;

            slotsVendor[i].CostText.gameObject.SetActive(true);
            slotsVendor[i].CostText.text = vendorInventorySlots[i].Item.Cost.ToString();
            slotsVendor[i].InventorySlot.SetSlot(vendorInventorySlots[i]);
        }
    }

    private void SetPlayerSlots()
    {
        var playerInventorySlots = Inventory.GetAllSlots();
        var filledSlotIndex = 0;
        for (int i = 0; i < playerInventorySlots.Length; i++)
        {
            slotsPlayer[i].CostText.gameObject.SetActive(false);

            if (playerInventorySlots[i].IsEmpty) continue;

            slotsPlayer[filledSlotIndex].InventorySlot.SetSlot(playerInventorySlots[i]);
            slotsPlayer[filledSlotIndex].CostText.gameObject.SetActive(true);
            slotsPlayer[filledSlotIndex].CostText.text =
                slotsPlayer[filledSlotIndex].InventorySlot.Slot.Item.Cost.ToString();
            filledSlotIndex++;
        }
    }


    public void SelectSlot(UIInventorySlot selectedSlot)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateSelectedSlot()
    {
        throw new System.NotImplementedException();
    }

    public void DeselectSlot()
    {
        throw new NotImplementedException();
    }
}
