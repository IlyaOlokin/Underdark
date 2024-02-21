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
        ApplySlow(receiver);
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        
        if (Timer <= 0)
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        ApplySlow(receiver);
        receiver.LooseStatusEffect(this);
    }
    
    private static void ApplySlow(Unit unit)
    {
        float slow = 0;
        foreach (var slowComp in unit.GetComponents<Slow>())
        {
            if (slowComp.Timer < 0) continue;
            if (slowComp.slowInfo.SlowAmount > slow)
                slow = slowComp.slowInfo.SlowAmount;
        }
        
        unit.ApplySlowDebuff(slow);
    }
}
