using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActiveAbilityInventoryUI : MonoBehaviour, IInventoryUI
{
    public Player Player { get; private set; }

    public Inventory Inventory { get; private set; }
    private UIInventorySlot selectedSlot;

    [SerializeField] private UIInventorySlot[] slots;
    [SerializeField] private UIInventorySlot[] equippedSlots;
    
    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;
    
    [Inject]
    private void Construct(Player player)
    {
        Player = player;
    }

    private void Awake()
    {
        Inventory = Player.Inventory;
        Inventory.OnInventoryChanged += UpdateUI;
        
        var inventorySlots = Inventory.GetAllActiveAbilitySlots();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            slots[i].SetSlot(inventorySlots[i], this);
        }
        
        for (int i = 0; i < Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            equippedSlots[i].SetSlot(Inventory.EquippedActiveAbilitySlots[i], this);
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

    public void UpdateSelectedSlot()
    {
        if (selectedSlot == null || selectedSlot.Slot.IsEmpty)
            itemDescription.ResetDescriptionActive(false);
        else
            itemDescription.ShowItemDescription(selectedSlot.Slot.Item, Player, selectedSlot.Slot);
    }
}
