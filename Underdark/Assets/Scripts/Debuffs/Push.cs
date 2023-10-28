using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : Debuff
{
    private IPushable receiver;

    
    public void Init(float pushDuration, IPushable receiver)
    {
        duration = pushDuration;
        this.receiver = receiver;
    }
    
    
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            receiver.EndPush();
            Destroy(this);
        }
    }
}
