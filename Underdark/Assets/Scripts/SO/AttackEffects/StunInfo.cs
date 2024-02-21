using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Stun", fileName = "New StunInfo")]

public class StunInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (Random.Range(0f, 1f) > chance) return;
        
        if (receiver.TryGetComponent(out Stun stunComponent))
        {
            stunComponent.AddDuration(Duration);
        }
        else
        {
            var newStun = receiver.gameObject.AddComponent<Stun>();
            newStun.Init(this, receiver, effectIcon);
            receiver.ReceiveStatusEffect(newStun);
        }
        
        receiver.GetStunned();
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance stuns the target for {Duration} seconds.";
    }
    
}
