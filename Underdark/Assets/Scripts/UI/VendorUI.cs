using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class VendorUI : InGameUiWindow, IInventoryUI
{
    public Inventory Inventory { get; private set; }
    [Header("Player Window")]
    [SerializeField] private UIInventorySlot[] slotsPlayer;
    
    [Header("Vendor Window")]
    private Vendor vendor;
    [SerializeField] private UIInventorySlot[] slotsVendor;

    
    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;

    public void Init(Vendor vendor)
    {
        this.vendor = vendor;
    }
    
    private void Awake()
    {
        base.Awake();
        Inventory = player.Inventory;
        
    }

    private void OnEnable()
    {
        SetSlots();

        
        Inventory.OnInventoryChanged += UpdateUI;

        UpdateUI();
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= UpdateUI;
    }
    
    private void SetSlots()
    {
        var playerInventorySlots = Inventory.GetAllSlots();
        var filledSlotIndex = 0;
        for (int i = 0; i < playerInventorySlots.Length; i++)
        {
            if (playerInventorySlots[i].IsEmpty)
            {
                slotsPlayer[filledSlotIndex].SetSlot(null);
            }
            else
            {
                slotsPlayer[filledSlotIndex].SetSlot(playerInventorySlots[i]);
                filledSlotIndex++;
            }
            
        }

        var vendorInventorySlots = vendor.Slots;
        for (int i = 0; i < vendorInventorySlots.Count; i++)
        {
            slotsVendor[i].SetSlot(vendorInventorySlots[i]);
        }
    }

    private void UpdateUI()
    {
        foreach (var slot in slotsPlayer)
        {
            slot.Refresh();
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
}
