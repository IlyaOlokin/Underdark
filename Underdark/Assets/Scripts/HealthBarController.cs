using System;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    protected void SetMaxHP(int maxHP)
    {
        slider.maxValue = maxHP;
    }
    
    protected void UpdateHealth(int currentHP)
    {
        slider.value = currentHP;
    }
}
