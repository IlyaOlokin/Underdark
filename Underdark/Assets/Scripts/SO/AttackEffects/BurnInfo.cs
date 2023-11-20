using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Brun", fileName = "New BurnInfo")]

public class BurnInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    public float BurnJumpDistance;
    [SerializeField] private GameObject visualPrefab;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        receiver.GetBurn(this, unitCaster, visualPrefab, effectIcon);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts burn on the target, dealing {Damage} damage per second for {Duration} seconds.";
    }
}
