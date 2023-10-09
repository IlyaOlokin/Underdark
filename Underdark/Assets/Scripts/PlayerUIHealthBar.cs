using Zenject;

public class PlayerUIHealthBar : HealthBarController
{
    [Inject]
    private void Cunstruct(Player player)
    {
        player.OnHealthChanged += UpdateHealth;
        player.OnMaxHealthChanged += SetMaxHP;
    }
}
