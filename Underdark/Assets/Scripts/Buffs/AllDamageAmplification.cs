using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllDamageAmplification : Buff
{
    private float dmgAmplification;

    public void Init(float duration, float dmgAmplification, Unit caster, Sprite buffIcon)
    {
        Duration = duration;
        Timer = duration;
        this.dmgAmplification = dmgAmplification;
        Icon = buffIcon;
        receiver = caster;

        receiver.Params.AddAllDamageAmplification(dmgAmplification);
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
        receiver.Params.AddAllDamageAmplification(-dmgAmplification);
        receiver.LooseStatusEffect(this);
    }
}
