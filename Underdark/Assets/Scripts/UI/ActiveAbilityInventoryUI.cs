using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ActiveAbilityInventoryUI : MonoBehaviour, IInventoryUI
{
    private Player player;
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private UIInventorySlot[] slots;
    [SerializeField] private UIInventorySlot[] equippedSlots;
    
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
    }
}
