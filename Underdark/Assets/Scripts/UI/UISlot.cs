using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IDropHandler
{
    public virtual void OnDrop(PointerEventData eventData)
    {
        var newItemTransform = eventData.pointerDrag.transform;
        newItemTransform.SetParent(transform);
        newItemTransform.localPosition = Vector3.zero;
    }
}