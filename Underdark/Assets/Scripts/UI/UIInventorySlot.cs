using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private UIInventoryItem uiInventoryItem;
    public IInventorySlot slot { get; private set; }
    [field: SerializeField] public ItemType SlotType { get; private set; }
    private IInventoryUI inventoryUI;

    private Vector2 pressPos;

    private void Awake()
    {
        inventoryUI = GetComponentInParent<IInventoryUI>();
        Refresh();
    }

    public void SetSlot(IInventorySlot newSlot)
    {
        slot = newSlot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIInventoryItem otherItemUI = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        UIInventorySlot otherSlotUI = otherItemUI.GetComponentInParent<UIInventorySlot>();

        var otherSlot = otherSlotUI.slot;
        var inventory = inventoryUI.Inventory;


        bool canMoveItem = false;
        if (SlotType == ItemType.Any && otherSlotUI.SlotType == ItemType.Any)
        {
            canMoveItem = true;
        }
        else if (SlotType == ItemType.Any && otherSlotUI.SlotType != ItemType.Any)
        {
            if (slot.IsEmpty || slot.Item.ItemType == otherSlotUI.SlotType)
                canMoveItem = true;
        }
        else if (otherSlotUI.SlotType == ItemType.Any && SlotType != ItemType.Any)
        {
            if (SlotType == otherItemUI.Item.ItemType)
                canMoveItem = true;
        }
        else if (SlotType != ItemType.Any && otherSlotUI.SlotType != ItemType.Any)
        {
            if (SlotType == otherItemUI.Item.ItemType && (slot.IsEmpty || slot.Item.ItemType == otherSlotUI.SlotType))
                canMoveItem = true;
        }

        if (canMoveItem) inventory.MoveItem(otherSlot, slot, otherSlotUI.SlotType, SlotType);

        Refresh();
        otherSlotUI.Refresh();
    }

    public void Refresh()
    {
        if (slot != null)
        {
            uiInventoryItem.Refresh(slot);
        }
    }

    public void OnSelect()
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    
    public void OnDeselect()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryUI.SelectSlot(this);
    }
}