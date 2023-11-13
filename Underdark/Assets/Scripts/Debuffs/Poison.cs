using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Debuff
{
    private GameObject currentVisualPrefab;
    private PoisonInfo poisonInfo;
    
    private float dmgTimer;
    
    public void Init(PoisonInfo poisonInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
    {
        this.poisonInfo = poisonInfo;
        this.receiver = receiver;
        base.caster = caster;
        Icon = effectIcon;
        dmgTimer = poisonInfo.DmgDelay;
        Duration = poisonInfo.Duration;
        Timer = Duration;

        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        Timer -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(caster, caster, poisonInfo.Damage, false);
            receiver.SpendMana(poisonInfo.Damage);
            dmgTimer = poisonInfo.DmgDelay;
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
