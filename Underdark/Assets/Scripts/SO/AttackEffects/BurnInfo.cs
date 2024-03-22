using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Brun", fileName = "New BurnInfo")]

public class BurnInfo : DebuffInfo
{
    public int Damage;
    public float DmgDelay;
    public float Duration;
    public float BurnJumpDistance;
    public float BurnJumpChance = 0.2f;
    [SerializeField] private GameObject visualPrefab;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (receiver.TryGetComponent(out Freeze freeze))
            Destroy(freeze);
        
        if (Random.Range(0f, 1f) > chance) return;
        if (receiver.TryGetComponent(out Burn burn)) return;
        
        var newBurn = receiver.gameObject.AddComponent<Burn>();
        newBurn.Init(this, receiver, unitCaster, visualPrefab, effectIcon);
        
        receiver.ReceiveStatusEffect(newBurn);
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts burn on the target, dealing {Damage} damage per second for {Duration} seconds.";
    }
}
