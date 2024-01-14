using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Zenject;

public class UnitLight : MonoBehaviour
{
    [SerializeField] private Unit unit;
    
    [Header("Lights")]
    [SerializeField] private Light2D defaultLight;
    [SerializeField] private Light2D bonusLight;

    private void OnEnable()
    {
        unit.Inventory.OnEquipmentChanged += UpdateLight;
    }

    private void Start()
    {
        UpdateLight();
    }

    private void UpdateLight()
    {
        var hasNightVision = unit.HasPassiveOfType<NightVisionSO>();
        
        defaultLight.enabled = !hasNightVision;
        bonusLight.enabled = hasNightVision;
    }
    
    private void OnDisable()
    {
        unit.Inventory.OnEquipmentChanged += UpdateLight;
    }
}
