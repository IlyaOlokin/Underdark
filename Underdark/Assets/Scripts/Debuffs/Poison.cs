using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Debuff
{
    
    private GameObject currentVisualPrefab;
    
    private PoisonInfo poisonInfo;
    private Unit receiver;
    
    private float dmgTimer;
    
    public void Init(PoisonInfo poisonInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
    {
        this.poisonInfo = poisonInfo;
        this.receiver = receiver;
        Caster = caster;
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
            receiver.TakeDamage(Caster, Caster, poisonInfo.Damage, false);
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
        receiver.LooseBuff(this);
        Destroy(currentVisualPrefab);
    }

    
}
