using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IDropHandler
{
   [SerializeField] private UIInventoryItem uiInventoryItem;
   public IInventorySlot slot { get; private set; }
   [field:SerializeField] public ItemType ItemType { get; private set; }
   private InventoryUI inventoryUI;

   private void Awake()
   {
      inventoryUI = GetComponentInParent<InventoryUI>();
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
      
      if (ItemType == ItemType.Any || ItemType == otherItemUI.Item.ItemType) 
         inventory.MoveItem(otherSlot, slot);
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
}
