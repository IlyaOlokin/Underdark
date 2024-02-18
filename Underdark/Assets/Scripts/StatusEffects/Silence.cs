using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silence : Debuff
{
    public void Init(float duration, Unit receiver, Sprite effectIcon)
    {
        Duration = duration;
        Timer = duration;
        this.receiver = receiver;
        Icon = effectIcon;
    }
    
    void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
        {
            receiver.EndSilence();
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        receiver.LooseStatusEffect(this);
    }
}
