using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DebuffInfos/Silence", fileName = "New SilenceInfo")]

public class SilenceInfo : DebuffInfo
{
    public float Duration;
    
    public override void Execute(IAttacker attacker, Unit receiver, Unit unitCaster)
    {
        if (Random.Range(0f, 1f) > chance) return;

        if (receiver.TryGetComponent(out Silence silenceComponent))
        {
            if (silenceComponent.Timer > Duration) return;
            
            receiver.EndSilence();
            Destroy(silenceComponent);
        }
        
        receiver.StartSilence();
        var newSilence = receiver.gameObject.AddComponent<Silence>();
        newSilence.Init(Duration, receiver, effectIcon);
        receiver.ReceiveStatusEffect(newSilence);
    }

    public override string ToString()
    {
        return
            $"With a {chance * 100}% chance inflicts silence on the target.";
    }
}
