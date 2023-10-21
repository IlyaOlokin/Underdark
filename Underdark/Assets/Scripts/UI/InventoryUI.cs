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

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        
    }

    private void Awake()
    {
        Inventory = player.Inventory;
        Inventory.OnInventoryChanged += UpdateUI;
        
        UpdateUI();

        var inventorySlots = Inventory.GetAllSlots();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            slots[i].SetSlot(inventorySlots[i]);
        }
    }

    private void UpdateUI()
    {
        //ClearSlots();
        
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
