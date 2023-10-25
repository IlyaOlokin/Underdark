using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Debuff
{
    private PoisonInfo poisonInfo;
    private IDamageable receiver;
    
    private float dmgTimer;
    
    public void Init(PoisonInfo poisonInfo, IDamageable receiver)
    {
        this.poisonInfo = poisonInfo;
        this.receiver = receiver;
        dmgTimer = poisonInfo.DmgDelay;
        duration = poisonInfo.Duration;
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        duration -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(null, poisonInfo.Damage, false);
            dmgTimer = poisonInfo.DmgDelay;
        }

        if (duration <= 0)
        {
            Destroy(this);
        }
    }
}
