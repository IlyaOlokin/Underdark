using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : Debuff
{
    private GameObject currentVisualPrefab;
    
    private BleedInfo bleedInfo;
    private Unit receiver;
    
    private float dmgTimer;
    
    public void Init(BleedInfo bleedInfo, Unit receiver, Unit caster, GameObject visual)
    {
        this.bleedInfo = bleedInfo;
        this.receiver = receiver;
        this.caster = caster;
        
        dmgTimer = bleedInfo.DmgDelay;
        duration = bleedInfo.Duration;
        
        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        duration -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(caster, caster, bleedInfo.Damage, false);
            dmgTimer = bleedInfo.DmgDelay;
        }

        if (duration <= 0)
        {
            Destroy(currentVisualPrefab);
            Destroy(this);
        }
    }
}
