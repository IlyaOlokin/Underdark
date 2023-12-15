using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private UIInventoryItem uiInventoryItem;
    public IInventorySlot Slot { get; private set; }
    [field: SerializeField] public ItemType SlotType { get; private set; }
    [SerializeField] private GameObject selectIndicator;
    public IInventoryUI InventoryUI { get; private set; }

    private Vector2 pressPos;

    private void Awake()
    {
        InventoryUI = GetComponentInParent<IInventoryUI>();
        Refresh();
    }

    public void SetSlot(IInventorySlot newSlot)
    {
        Slot = newSlot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIInventoryItem otherItemUI = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        if (!otherItemUI.Draggable) return;
        UIInventorySlot otherSlotUI = otherItemUI.GetComponentInParent<UIInventorySlot>();

        var otherSlot = otherSlotUI.Slot;
        var inventory = InventoryUI.Inventory;


        bool canMoveItem = false;
        if (SlotType == ItemType.Any && otherSlotUI.SlotType == ItemType.Any)
        {
            canMoveItem = true;
        }
        else if (SlotType == ItemType.Any && otherSlotUI.SlotType != ItemType.Any)
        {
            if (Slot.IsEmpty || Slot.Item.ItemType == otherSlotUI.SlotType)
                canMoveItem = true;
        }
        else if (otherSlotUI.SlotType == ItemType.Any && SlotType != ItemType.Any)
        {
            if (SlotType == otherItemUI.Item.ItemType)
                canMoveItem = true;
        }
        else if (SlotType != ItemType.Any && otherSlotUI.SlotType != ItemType.Any)
        {
            if (SlotType == otherItemUI.Item.ItemType && (Slot.IsEmpty || Slot.Item.ItemType == otherSlotUI.SlotType))
                canMoveItem = true;
        }

        if (canMoveItem) inventory.MoveItem(otherSlot, Slot, otherSlotUI.SlotType, SlotType);

        //Refresh();
        //otherSlotUI.Refresh();
    }

    public void Refresh()
    {
        uiInventoryItem.Refresh(Slot);
    }

    public void OnSelect()
    {
        selectIndicator.SetActive(true);
    }
    
    public void OnDeselect()
    {
        selectIndicator.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InventoryUI.SelectSlot(this);
    }
}