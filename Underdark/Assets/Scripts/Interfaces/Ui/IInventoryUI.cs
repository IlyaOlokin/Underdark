using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryUI
{
    Inventory Inventory { get; }

    void SelectSlot(UIInventorySlot selectedSlot);
}
