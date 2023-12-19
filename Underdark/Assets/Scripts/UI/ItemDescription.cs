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
