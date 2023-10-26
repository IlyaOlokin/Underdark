using Zenject;

public class PlayerUIHealthBar : BarController
{
    [Inject]
    private void Cunstruct(Player player)
    {
        player.OnHealthChanged += UpdateValue;
        player.OnMaxHealthChanged += SetMaxValue;
    }
}
