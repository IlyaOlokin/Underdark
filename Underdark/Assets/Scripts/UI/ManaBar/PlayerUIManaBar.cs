using Zenject;

public class PlayerUIManaBar : BarController
{
    [Inject]
    private void Cunstruct(Player player)
    {
        player.OnManaChanged += UpdateValue;
        player.OnMaxManaChanged += SetMaxValue;
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
    }
}
