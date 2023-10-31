using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExecutableSlotsHandler : MonoBehaviour
{
    public Button[] executableButtons;
    private List<IInventorySlot> executableSlots;
    private Player player;
    
    [SerializeField] private UIInventoryItem[] items;
    
    public void Init(Player player)
    {
        this.player = player;
        executableSlots = new List<IInventorySlot>();
        for (int i = 0; i < player.Inventory.ExecutableSlots.Count; i++)
        {
            executableSlots.Add(player.Inventory.ExecutableSlots[i]);
        }

        for (int i = 0; i < executableSlots.Count; i++)
        {
            RefreshSlot(i);
        }
        player.OnExecutableItemUse += RefreshSlot;
    }

    private void OnDisable()
    {
        player.OnExecutableItemUse -= RefreshSlot;
    }

    private void RefreshSlot(int index)
    {
        items[index].Refresh(executableSlots[index]);
    }

    public void RefreshAllSlots()
    {
        for (int i = 0; i < executableSlots.Count; i++)
        {
            items[i].Refresh(executableSlots[i]);
        }
    }
}
