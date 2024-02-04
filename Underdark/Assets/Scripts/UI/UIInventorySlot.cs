using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UIInventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private UIInventoryItem uiInventoryItem;
    public IInventorySlot Slot { get; private set; }
    [field: SerializeField] public ItemType SlotType { get; private set; }
    [SerializeField] private GameObject selectIndicator;
    public IInventoryUI InventoryUI { get; private set; }

    public void SetSlot(IInventorySlot newSlot, IInventoryUI inventoryUI)
    {
        Slot = newSlot;
        if (newSlot != null)
            Slot.SlotType = SlotType;

        InventoryUI = inventoryUI;
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIInventoryItem otherItemUI = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        if (!otherItemUI.Draggable) return;
        UIInventorySlot otherSlotUI = otherItemUI.GetComponentInParent<UIInventorySlot>();

        var otherSlot = otherSlotUI.Slot;
        var inventory = InventoryUI.Inventory;
        
        inventory.TryMoveItem(otherSlot, Slot, otherSlotUI.SlotType, SlotType);
    }

    public void Refresh()
    {
        uiInventoryItem.Refresh(Slot, InventoryUI.Player.ActiveAbilitiesExp);
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