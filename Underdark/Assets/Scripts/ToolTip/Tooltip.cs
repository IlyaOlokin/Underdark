using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPanel;
    private bool isPanelActive;
    
    private void Update()
    {
        if (tooltipPanel.activeInHierarchy && Input.anyKeyDown)
        {
            tooltipPanel.SetActive(false);
        }
    }

    public void ToggleTooltip()
    {
        tooltipPanel.SetActive(!isPanelActive);
        isPanelActive = !isPanelActive;
    }
}
