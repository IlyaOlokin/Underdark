using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

public class InventoryUI : MonoBehaviour, IInventoryUI
{
    private Player player;
    public Inventory Inventory { get; private set; }
    private UIInventorySlot selectedSlot;

    [Header("Slots")]
    [SerializeField] private UIInventorySlot[] slots;
    
    [SerializeField] private UIInventorySlot head;
    [SerializeField] private UIInventorySlot body;
    [SerializeField] private UIInventorySlot legs;
    [SerializeField] private UIInventorySlot weapon;
    [SerializeField] private UIInventorySlot shield;
    
    [SerializeField] private UIInventorySlot[] executableSlots;

    [Header("Stats Text")] 
    [SerializeField] private TextMeshProUGUI attackText; 
    [SerializeField] private TextMeshProUGUI armorText;

    [Header("Item Description")] 
    [SerializeField] private ItemDescription itemDescription;
    
    [NonSerialized] public GameObject blackOut;
    
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
        SetExecutableSlots();
    }

    private void SetExecutableSlots()
    {
        for (int i = 0; i < Inventory.ExecutableSlots.Count; i++)
        {
            executableSlots[i].SetSlot(Inventory.ExecutableSlots[i]);
        }
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
        blackOut.SetActive(true);
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

        foreach (var executableSlot in executableSlots)
        {
            executableSlot.Refresh();
        }
        
        UpdateSelectedSlot();

        armorText.text = player.GetTotalArmor().ToString();
        attackText.text = player.GetTotalDamage().ToString();
    }

    private void OnDisable()
    {
        blackOut.SetActive(false);
        DeselectSlot();
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
            itemDescription.ShowItemDescription(selectedSlot.slot.Item, player);
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

    private void ClearSlots()
    {
        foreach (var slot in slots)
        {
            //slot.Clear();
        }
    }
}
