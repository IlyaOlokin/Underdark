using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventorySlot : UISlot
{
   [SerializeField] private UIInventoryItem uiInventoryItem;
   public IInventorySlot slot { get; private set; }
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
   
   public override void OnDrop(PointerEventData eventData)
   {
      UIInventoryItem otherItemUI = eventData.pointerDrag.GetComponent<UIInventoryItem>();
      UIInventorySlot otherSlotUI = otherItemUI.GetComponentInParent<UIInventorySlot>();
      
      base.OnDrop(eventData);
      
      var otherSlot = otherSlotUI.slot;
      var inventory = inventoryUI.Inventory;
      
      inventory.MoveItem(otherSlot, slot);
      Refresh();
      otherSlotUI.Refresh();
   }

   private void Refresh()
   {
      if (slot != null)
      {
         uiInventoryItem.Refresh(slot);
      }
   }
}
