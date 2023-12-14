using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Vendor : MonoBehaviour
{
    public List<InventorySlot> Slots = new ();
    [SerializeField] private List<Item> items;
    private VendorUI vendorUI;

    [Inject]
    private void Construct(VendorUI vendorUI)
    {
        this.vendorUI = vendorUI;

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Slots.Add(new InventorySlot());
            Slots[i].SetItem(items[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            vendorUI.Init(this);
            vendorUI.OpenWindow();
        }
    }
}
