using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : Debuff
{
    public void Init(float pushDuration, Unit receiver, Sprite effectIcon)
    {
        Duration = pushDuration;
        Timer = pushDuration;
        this.receiver = receiver;
        Icon = effectIcon;
    }
    
    
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        receiver.EndPush();
        receiver.LooseStatusEffect(this);
    }
}
