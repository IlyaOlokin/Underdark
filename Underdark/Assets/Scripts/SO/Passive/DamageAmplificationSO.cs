using UnityEngine;

[CreateAssetMenu(menuName = "Passive/DamageAmplification", fileName = "New Damage Amplification")]
public class DamageAmplificationSO : PassiveSO
{
    public DamageType DamageType;
    public float Value;
    
    public override string ToString()
    {
        return $"Increases outgoing {DamageType} damage by {Value * 100}%.";
    }
}
