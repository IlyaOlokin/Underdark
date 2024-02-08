using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryUI
{
    Player Player { get; }
    
    Inventory Inventory { get; }

    void SelectSlot(UIInventorySlot selectedSlot);
    void UpdateSelectedSlot();
}
