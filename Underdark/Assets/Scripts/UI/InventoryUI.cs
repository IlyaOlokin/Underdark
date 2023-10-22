using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InventoryUI : MonoBehaviour
{
    private Player player;
    public Inventory Inventory { get; private set; }
    [SerializeField] protected UIInventorySlot[] slots;
    
    public void Init(Player player)
    {
        this.player = player;
    }

    private void Awake()
    {
        Inventory = player.Inventory;
        Inventory.OnInventoryChanged += UpdateUI;
        
        var inventorySlots = Inventory.GetAllSlots();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            slots[i].SetSlot(inventorySlots[i]);
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
    }

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            //slot.Clear();
        }
    }

    public enum MoveResult
    {
        Success,
        Fail,
        SameItem,
        SameItemOverflow
    }
}
