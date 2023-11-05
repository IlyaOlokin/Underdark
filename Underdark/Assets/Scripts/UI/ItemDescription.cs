using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemName;
    
    public void ShowItemDescription(Item item)
    {
        SetDescriptionActive(true);
        
        icon.sprite = item.Sprite;
        itemName.text = item.name;
    }

    public void SetDescriptionActive(bool enabled)
    {
        icon.gameObject.SetActive(enabled);
        itemName.gameObject.SetActive(enabled);
    }
}
