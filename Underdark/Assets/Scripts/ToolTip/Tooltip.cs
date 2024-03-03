using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject tooltipPanel;
    private bool isPanelActive;
    
    private void Update()
    {
        if (tooltipPanel.activeInHierarchy && Input.anyKeyDown)
        {
            tooltipPanel.SetActive(false);
            isPanelActive = !isPanelActive;

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tooltipPanel.activeInHierarchy)
            isPanelActive = !isPanelActive;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tooltipPanel.SetActive(!isPanelActive);
        isPanelActive = !isPanelActive;
    }
}
