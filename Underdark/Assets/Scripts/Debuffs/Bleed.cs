using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Debuff
{
    private BleedInfo bleedInfo;
    private Unit receiver;
    
    private float dmgTimer;
    
    public void Init(BleedInfo bleedInfo, Unit receiver)
    {
        this.bleedInfo = bleedInfo;
        this.receiver = receiver;
        dmgTimer = bleedInfo.DmgDelay;
        duration = bleedInfo.Duration;
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        duration -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(null, bleedInfo.Damage, false);
            dmgTimer = bleedInfo.DmgDelay;
        }

        if (duration <= 0)
        {
            Destroy(this);
        }
    }
}
