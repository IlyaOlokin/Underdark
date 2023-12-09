using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGearUI : MonoBehaviour
{
    [Header("Tab Buttons")] 
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button paramsButton;
    
    [Header("Tab")] 
    [SerializeField] private InventoryUI inventoryTab;
    [SerializeField] private ParamsUI paramsTab;
    
    [NonSerialized] public GameObject blackOut;

    private void Awake()
    {
        paramsButton.onClick.AddListener(ActivateParamsTab);
        inventoryButton.onClick.AddListener(ActivateInventoryTab);
    }

    private void OnEnable()
    {
        ActivateInventoryTab();
        blackOut.SetActive(true); 
        transform.SetAsLastSibling();
    }

    private void OnDisable()
    {
        blackOut.SetActive(false);
    }

    private void ActivateInventoryTab()
    {
        paramsTab.gameObject.SetActive(false);
        paramsButton.interactable = true;
        
        inventoryTab.gameObject.SetActive(true);
        inventoryButton.interactable = false;
    }
    
    private void ActivateParamsTab()
    {
        inventoryTab.gameObject.SetActive(false);
        inventoryButton.interactable = true;

        paramsTab.gameObject.SetActive(true);
        paramsButton.interactable = false;
    }
}
