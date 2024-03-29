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
    [SerializeField] private GameObject nonValidSlotIndicator;
    
    public Item Item { get; private set; }

    public void Refresh(IInventorySlot slot, Unit owner)
    {
        if (slot == null || slot.IsEmpty)
        {
            Hide();
            return;
        }

        Item = slot.Item;
        var item = slot.Item;
        icon.sprite = item.Sprite;
        icon.gameObject.SetActive(true);
        
        if (slot.Item.ItemType == ItemType.ActiveAbility)
        {
            var activeAbility = ((ActiveAbilitySO)slot.Item).ActiveAbility;
            text.text = RomanConverter.NumberToRoman(activeAbility.ActiveAbilityLevelSetupSO
                .GetCurrentLevel(owner.GetExpOfActiveAbility(activeAbility.ID)));
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(slot.Amount > 1);
            text.text = slot.Amount.ToString();
        }

        nonValidSlotIndicator.SetActive(!slot.IsValid);
    }

    private void Hide()
    {
        icon.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        nonValidSlotIndicator.SetActive(false);
    }
}
