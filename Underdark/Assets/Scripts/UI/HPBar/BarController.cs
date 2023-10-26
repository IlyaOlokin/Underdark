using System;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class BarController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    
    protected void SetMaxValue(int maxValue)
    {
        slider.maxValue = maxValue;
    }
    
    protected void UpdateValue(int currentValue)
    {
        slider.value = currentValue;
    }
}
