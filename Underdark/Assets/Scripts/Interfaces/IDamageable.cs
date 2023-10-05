public interface IDamageable
{
    int MaxHP { get; }
    int CurrentHP { get; }
    HealthBarController HPBar { get; }
    
    void TakeDamage(int damage);
}