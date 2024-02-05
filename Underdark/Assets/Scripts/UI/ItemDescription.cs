using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private List<TextMeshProUGUI> propertyFields;

    [Header("Additional Properties")]
    [SerializeField] private TextMeshProUGUI additionalPropertyFieldPref;
    [SerializeField] private Transform additionalPropertyFieldInitialPos;
    [SerializeField] private Transform additionalPropertyFieldParents;
    private List<TextMeshProUGUI> additionalPropertyFields = new List<TextMeshProUGUI>();

    [Header("Buttons")]
    [SerializeField] private Button useItemButton;
    [SerializeField] private Button deleteItemButton;
    [SerializeField] private Button confirmDeleteItemButton;
    [SerializeField] private Button canselDeleteItemButton;
    
    [Header("Other")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private AbilityLevelDisplayUI abilityLevelDisplay;
    
    private Unit currOwner;
    private IInventorySlot currInventorySlot;

    private void Awake()
    {
        useItemButton.onClick.AddListener(UseItem);
        
        deleteItemButton.onClick.AddListener(OpenConfirmPanel);
        confirmDeleteItemButton.onClick.AddListener(DeleteItem);
        confirmDeleteItemButton.onClick.AddListener(CloseConfirmPanel);
        canselDeleteItemButton.onClick.AddListener(CloseConfirmPanel);
    }

    public void ShowItemDescription(Item item, Unit owner, IInventorySlot slot)
    {
        ResetDescriptionActive(true);
        
        useItemButton.interactable = slot.SlotType is ItemType.Any or ItemType.Executable;
        
        currOwner = owner;
        currInventorySlot = slot;
        deleteItemButton.interactable = true;
        
        icon.sprite = item.Sprite;
        itemName.text = item.Name;

        var properties = item.ToString(owner);

        for (int i = 0; i < properties.Length; i++)
        {
            propertyFields[i].gameObject.SetActive(true);
            propertyFields[i].text = properties[i];
        }

        var additionalProperties = item.ToStringAdditional();
        
        for (int i = 0; i < additionalProperties.Length; i++)
        {
            var newText = Instantiate(additionalPropertyFieldPref, additionalPropertyFieldInitialPos.position, Quaternion.identity, additionalPropertyFieldParents);
            
            additionalPropertyFields.Add(newText);
            newText.text = additionalProperties[i];
            newText.ForceMeshUpdate();
            if (i == 0) continue;
            
            var renderedValues = additionalPropertyFields[i - 1].GetRenderedValues(true);
            var yOffset = (renderedValues.y + 10) / 6f;
            var lastPropertyPos = additionalPropertyFields[i - 1].rectTransform.anchoredPosition;
            
            newText.rectTransform.anchoredPosition = new Vector3(lastPropertyPos.x, lastPropertyPos.y - yOffset, 0);
        }

        if (item.ItemType == ItemType.Executable && ((ExecutableItemSO)item).ExecutableItem is ScrollActiveAbility)
        {
            abilityLevelDisplay.gameObject.SetActive(true);

            var scroll = (ScrollActiveAbility)((ExecutableItemSO)item).ExecutableItem;
            abilityLevelDisplay.DisplayAbilityLevel(scroll.Item, owner.GetExpOfActiveAbility(scroll.Item.ID));
        }
        else if (item.ItemType == ItemType.ActiveAbility)
        {
            abilityLevelDisplay.gameObject.SetActive(true);
            
            abilityLevelDisplay.DisplayAbilityLevel((ActiveAbilitySO)item, owner.GetExpOfActiveAbility(item.ID));
        }
    }

    public void ResetDescriptionActive(bool enabled)
    {
        useItemButton.interactable = false;
        deleteItemButton.interactable = false;
        
        foreach (var propertyField in propertyFields)
        {
            propertyField.gameObject.SetActive(false);
        }

        foreach (var additionalProperty in additionalPropertyFields)
        {
            Destroy(additionalProperty.gameObject);
        }
        additionalPropertyFields.Clear();

        icon.gameObject.SetActive(enabled);
        itemName.gameObject.SetActive(enabled);

        CloseConfirmPanel();
        abilityLevelDisplay.gameObject.SetActive(false);
    }

    private void UseItem()
    {
        var item = currInventorySlot.Item;
        IInventorySlot targetSlot = currInventorySlot;
        
        switch (item.ItemType)
        {
            case ItemType.Head:
                targetSlot = currOwner.Inventory.Equipment.Head;
                break;
            case ItemType.Body:
                targetSlot = currOwner.Inventory.Equipment.Body;
                break;
            case ItemType.Legs:
                targetSlot = currOwner.Inventory.Equipment.Legs;
                break;
            case ItemType.Weapon:
                targetSlot = currOwner.Inventory.Equipment.Weapon;
                break;
            case ItemType.Shield:
                targetSlot = currOwner.Inventory.Equipment.Shield;
                break;
            case ItemType.Accessory:
                targetSlot = currOwner.Inventory.Equipment.Accessories[0];
                foreach (var accessorySlot in currOwner.Inventory.Equipment.Accessories)
                {
                    if (accessorySlot.IsEmpty)
                    {
                        targetSlot = accessorySlot;
                        break;
                    }
                }
                
                break;
            case ItemType.Executable:
                if (((ExecutableItemSO)item).Execute(currOwner))
                    currOwner.Inventory.Remove(currInventorySlot);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        currOwner.Inventory.TryMoveItem(currInventorySlot, targetSlot,
            currInventorySlot.SlotType, targetSlot.SlotType);
    }

    private void DeleteItem()
    {
        currOwner.Inventory.ClearSlot(currInventorySlot);
    }

    private void OpenConfirmPanel()
    {
        confirmPanel.SetActive(true);
    }
    
    private void CloseConfirmPanel()
    {
        confirmPanel.SetActive(false);
    }
}
