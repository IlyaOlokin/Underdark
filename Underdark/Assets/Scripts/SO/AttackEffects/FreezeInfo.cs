using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Freeze", fileName = "New FreezeInfo")]

public class FreezeInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (receiver.TryGetComponent(out Burn burn))
            Destroy(burn);
        
        if (Random.Range(0f, 1f) > chance) return;
        
        if (receiver.TryGetComponent(out Freeze stunComponent))
        {
            stunComponent.AddDuration(Duration);
        }
        else
        {
            var newStun = receiver.gameObject.AddComponent<Freeze>();
            newStun.Init(this, receiver, effectIcon);
            receiver.ReceiveStatusEffect(newStun);
        }
        
        receiver.GetFrozen();
    }
    
    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance freeze the target for {Duration} seconds.";
    }
    
}
