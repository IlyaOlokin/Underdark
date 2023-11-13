using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Debuff
{
    private GameObject currentVisualPrefab;
    
    private BleedInfo bleedInfo;
    private Unit receiver;
    
    private float dmgTimer;
    
    public void Init(BleedInfo bleedInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
    {
        this.bleedInfo = bleedInfo;
        this.receiver = receiver;
        Caster = caster;
        Icon = effectIcon;
        dmgTimer = bleedInfo.DmgDelay;
        Duration = bleedInfo.Duration;
        Timer = Duration;

        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(Caster, Caster, bleedInfo.Damage, false);
            dmgTimer = bleedInfo.DmgDelay;
        }

        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        receiver.LooseBuff(this);
        Destroy(currentVisualPrefab);
    }
}
