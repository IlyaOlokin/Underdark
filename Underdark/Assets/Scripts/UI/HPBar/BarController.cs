using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class BarController : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI currentValueText;
    [SerializeField] protected TextMeshProUGUI maxValueText;
    [SerializeField] protected Slider slider;
    
    protected virtual void SetMaxValue(int maxValue)
    {
        slider.maxValue = maxValue;
    }
    
    protected virtual void UpdateValue(int currentValue)
    {
        slider.value = currentValue;
    }
}
