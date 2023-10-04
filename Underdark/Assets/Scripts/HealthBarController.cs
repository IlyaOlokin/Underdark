using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxHP(int maxHP)
    {
        slider.maxValue = maxHP;
    }
    
    public void UpdateHealth(int currentHP)
    {
        slider.value = currentHP;
    }
}
