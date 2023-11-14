using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActiveAbilityInventoryUI : MonoBehaviour, IInventoryUI
{
    private Player player;
    public Inventory Inventory { get; private set; }
    private UIInventorySlot selectedSlot;

    [SerializeField] private UIInventorySlot[] slots;
    [SerializeField] private UIInventorySlot[] equippedSlots;
    
    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    private void Awake()
    {
        Inventory = player.Inventory;
        Inventory.OnInventoryChanged += UpdateUI;
        
        var inventorySlots = Inventory.GetAllActiveAbilitySlots();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            slots[i].SetSlot(inventorySlots[i]);
        }
        
        for (int i = 0; i < Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            equippedSlots[i].SetSlot(Inventory.EquippedActiveAbilitySlots[i]);
        }
    }
    
    private void OnEnable()
    {
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        foreach (var slot in slots)
        {
            slot.Refresh();
        }
        foreach (var slot in equippedSlots)
        {
            slot.Refresh();
        }
        
        UpdateSelectedSlot();
    }
    
    public void SelectSlot(UIInventorySlot selectedSlot)
    {
        if (this.selectedSlot != null)
            this.selectedSlot.OnDeselect();
        
        this.selectedSlot = selectedSlot;
        this.selectedSlot.OnSelect();

        UpdateSelectedSlot();
    }

    private void UpdateSelectedSlot()
    {
        if (selectedSlot == null || selectedSlot.slot.IsEmpty)
            itemDescription.ResetDescriptionActive(false);
        else
            itemDescription.ShowItemDescription(selectedSlot.slot.Item, player, selectedSlot.slot);
    }

    private void DeselectSlot()
    {
        if (selectedSlot == null)
        {
            return;
        }
        selectedSlot.OnDeselect();
        selectedSlot = null;
    }
}
