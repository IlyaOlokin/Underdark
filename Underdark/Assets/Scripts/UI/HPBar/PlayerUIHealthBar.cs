using System;
using Zenject;

public class PlayerUIHealthBar : BarController
{
    public event Action<float> OnValueChanged;
    
    [Inject]
    private void Cunstruct(Player player)
    {
        player.OnHealthChanged += UpdateValue;
        player.OnMaxHealthChanged += SetMaxValue;
    }
    
    protected override void SetMaxValue(int maxValue)
    {
        base.SetMaxValue(maxValue);
        maxValueText.text = $@"/{maxValue}";
    }
    
    protected override void UpdateValue(int currentValue)
    {
        base.UpdateValue(currentValue);
        currentValueText.text = currentValue.ToString();
        OnValueChanged?.Invoke(currentValue);
    }
}
