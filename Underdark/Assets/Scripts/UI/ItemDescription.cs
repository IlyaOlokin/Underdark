using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> propertyFields;
    [SerializeField] private List<TextMeshProUGUI> additionalPropertyFields;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;

    [SerializeField] private Button useItemButton;
    [SerializeField] private Button deleteItemButton;
    [SerializeField] private Button confirmDeleteItemButton;
    [SerializeField] private Button canselDeleteItemButton;
    
    [SerializeField] private GameObject confirmPanel;

    private ExecutableItemSO currExecutableItem;
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
        
        if (item.GetType() == typeof(ExecutableItemSO))
        {
            currExecutableItem = (ExecutableItemSO) item;
            useItemButton.interactable = true;
        }
        
        currOwner = owner;
        currInventorySlot = slot;
        deleteItemButton.interactable = true;
        
        icon.sprite = item.Sprite;
        itemName.text = item.Name;

        var properties = item.ItemType == ItemType.Executable ? item.ToString(owner) : item.ToString();

        for (int i = 0; i < properties.Length; i++)
        {
            propertyFields[i].gameObject.SetActive(true);
            propertyFields[i].text = properties[i];
        }

        var additionalProperties = item.ToStringAdditional();
        
        for (int i = 0; i < additionalProperties.Length; i++)
        {
            additionalPropertyFields[i].gameObject.SetActive(true);
            additionalPropertyFields[i].text = additionalProperties[i];
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
            additionalProperty.gameObject.SetActive(false);
        }

        icon.gameObject.SetActive(enabled);
        itemName.gameObject.SetActive(enabled);

        CloseConfirmPanel();
    }

    private void UseItem()
    {
        if (!currExecutableItem.Execute(currOwner)) return;

        currOwner.Inventory.Remove(currInventorySlot);
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
