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
    
    [SerializeField] protected UIInventorySlot head;
    [SerializeField] protected UIInventorySlot body;
    [SerializeField] protected UIInventorySlot legs;
    [SerializeField] protected UIInventorySlot weapon;
    [SerializeField] protected UIInventorySlot shield;
    
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

        SetEquipmentSlots();
    }

    private void SetEquipmentSlots()
    {
        var equipmentSlots = Inventory.Equipment;
        head.SetSlot(equipmentSlots.Head);
        body.SetSlot(equipmentSlots.Body);
        legs.SetSlot(equipmentSlots.Legs);
        weapon.SetSlot(equipmentSlots.Weapon);
        shield.SetSlot(equipmentSlots.Shield);
    }

    private void OnEnable()
    {
        UpdateUI();
        transform.SetAsLastSibling();
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
}
