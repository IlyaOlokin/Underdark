using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Debuff
{
    private SlowInfo slowInfo;
    
    public void Init(SlowInfo slowInfo, Unit receiver, Sprite effectIcon)
    {
        this.slowInfo = slowInfo;
        this.receiver = receiver;
        Icon = effectIcon;
        
        Duration = slowInfo.Duration;
        Timer = Duration;
        ApplySlow();
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        
        if (Timer <= 0)
        {
            Destroy(this);
        }
    }
    
    private void ApplySlow()
    {
        float slow = 1;
        foreach (var slowComp in receiver.GetComponents<Slow>())
        {
            if (slowComp.Timer < 0) continue;
            if (slowComp.slowInfo.SlowAmount > slow)
                slow = slowComp.slowInfo.SlowAmount;
        }
        
        receiver.ApplySlow(slow);
    }
    

    private void OnDestroy()
    {
        ApplySlow();
        receiver.LooseStatusEffect(this);
    }
}
