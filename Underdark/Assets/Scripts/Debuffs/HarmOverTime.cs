using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HarmOverTime : Debuff
{
    public HarmInfo HarmInfo { get; private set; }

    private GameObject currentVisualPrefab;
    private DamageInfo damageInfo = new();
    
    private float dmgTimer;
    
    public void Init(HarmInfo harmInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
    {
        this.HarmInfo = harmInfo;
        this.receiver = receiver;
        base.caster = caster;
        Icon = effectIcon;
        dmgTimer = harmInfo.DmgDelay;
        Duration = harmInfo.Duration;
        Timer = Duration;
        
        damageInfo.AddDamage(this.HarmInfo.Damage, multiplier: caster.Params.GetDamageAmplification(DamageType.Physic));

        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            if (HarmInfo.Damage != 0) receiver.TakeDamage(caster, null, damageInfo, false, 1f);
            if (HarmInfo.ManaDrainAmount != 0) receiver.SpendMana(HarmInfo.ManaDrainAmount);
            dmgTimer = HarmInfo.DmgDelay;
        }

        if (Timer <= 0)
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        receiver.LooseStatusEffect(this);
        Destroy(currentVisualPrefab);
    }

    
}
