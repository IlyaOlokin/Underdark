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
        receiver.StartPushState();
    }
    
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            receiver.EndPushState();
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        receiver.LooseStatusEffect(this);
    }
}
