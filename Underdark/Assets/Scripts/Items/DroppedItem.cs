using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Item containedItem;
    [SerializeField] private int itemAmount;

    private void Awake()
    {
        SetDroppedItem(containedItem, itemAmount);
    }

    public void SetDroppedItem(Item item, int amount)
    {
        containedItem = item;
        itemAmount = amount;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPickUper pickUper))
        {
            if (pickUper.TryPickUpItem(containedItem, itemAmount))
            {
                Destroy(gameObject);
            }
        }
    }
}
