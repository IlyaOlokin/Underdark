using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> propertyFields;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    
    public void ShowItemDescription(Item item)
    {
        SetDescriptionActive(true);
        
        icon.sprite = item.Sprite;
        itemName.text = item.name;

        var properties = item.ToString();

        for (int i = 0; i < properties.Length; i++)
        {
            propertyFields[i].gameObject.SetActive(true);
            propertyFields[i].text = properties[i];
        }
    }

    public void SetDescriptionActive(bool enabled)
    {
        foreach (var propertyField in propertyFields)
        {
            propertyField.gameObject.SetActive(false);
        }
        icon.gameObject.SetActive(enabled);
        itemName.gameObject.SetActive(enabled);
    }
}
