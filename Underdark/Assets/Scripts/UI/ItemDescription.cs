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

    private ExecutableItemSO currItem;
    private Unit currOwner;
    private IInventorySlot currInventorySlot;
    
    public void ShowItemDescription(Item item, Unit owner, IInventorySlot slot)
    {
        ResetDescriptionActive(true);
        
        if (item.GetType() == typeof(ExecutableItemSO))
        {
            currItem = (ExecutableItemSO) item;
            currOwner = owner;
            currInventorySlot = slot;
            useItemButton.onClick.AddListener(UseItem);
            useItemButton.interactable = true;
        }
        
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
        useItemButton.onClick.RemoveAllListeners();
        useItemButton.interactable = false;
        
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
    }

    private void UseItem()
    {
        currItem.Execute(currOwner);
        currOwner.Inventory.Remove(currInventorySlot);
    }
}
