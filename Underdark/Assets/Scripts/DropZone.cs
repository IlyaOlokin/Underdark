using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Drop))]
public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private Drop drop;
    [SerializeField] private int dropForce;
    
    public void OnDrop(PointerEventData eventData)
    {
        UIInventoryItem otherItemUI = eventData.pointerDrag.GetComponent<UIInventoryItem>();
        UIInventorySlot otherSlotUI = otherItemUI.GetComponentInParent<UIInventorySlot>();
        
        var otherSlot = otherSlotUI.slot;
        drop.DropItem(otherSlot.Item, otherSlot.Amount, Camera.main.transform.position, dropForce, true);
        
        otherSlot.Clear();
        otherSlotUI.Refresh();
        otherSlotUI.InventoryUI.UpdateSelectedSlot();
    }
}
