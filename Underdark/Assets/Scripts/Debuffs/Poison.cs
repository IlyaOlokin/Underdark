using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Debuff
{
    private GameObject currentVisualPrefab;
    
    private PoisonInfo poisonInfo;
    private Unit receiver;
    
    private float dmgTimer;
    
    public void Init(PoisonInfo poisonInfo, Unit receiver, Unit caster, GameObject visual)
    {
        this.poisonInfo = poisonInfo;
        this.receiver = receiver;
        this.caster = caster;

        dmgTimer = poisonInfo.DmgDelay;
        duration = poisonInfo.Duration;

        currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
    }
    public void Update()
    {
        dmgTimer -= Time.deltaTime;
        duration -= Time.deltaTime;
        if (dmgTimer <= 0)
        {
            receiver.TakeDamage(caster, poisonInfo.Damage, false);
            receiver.SpendMana(poisonInfo.Damage);
            dmgTimer = poisonInfo.DmgDelay;
        }

        if (duration <= 0)
        {
            Destroy(currentVisualPrefab);
            Destroy(this);
        }
    }
}
