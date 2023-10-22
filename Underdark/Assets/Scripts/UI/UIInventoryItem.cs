using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : UIItem
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    
    public Item Item { get; private set; }

    public void Refresh(IInventorySlot slot)
    {
        if (slot.IsEmpty)
        {
            Hide();
            return;
        }

        Item = slot.Item;
        var item = slot.Item;
        icon.sprite = item.sprite;
        icon.gameObject.SetActive(true);
        text.gameObject.SetActive(slot.Amount > 1);
        text.text = slot.Amount.ToString();
    }

    private void Hide()
    {
        icon.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }
}
