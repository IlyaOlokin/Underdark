using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Debuff
{
    private StunInfo stunInfo;
    private IStunable receiver;
    
    private float duration;

    public void Init(StunInfo stunInfo, IStunable receiver)
    {
        duration = stunInfo.Duration;
        this.receiver = receiver;
    }
    
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            receiver.GetUnStunned();
            Destroy(this);
        }
    }

    public void AddDuration(float addDuration)
    {
        duration += addDuration;
    }
}
