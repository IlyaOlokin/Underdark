using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDrain : Debuff
{
private GameObject currentVisualPrefab;
private ManaDrainInfo poisonInfo;
private DamageInfo damageInfo = new();

    
private float dmgTimer;
    
public void Init(ManaDrainInfo poisonInfo, Unit receiver, Unit caster, GameObject visual, Sprite effectIcon)
{
    this.poisonInfo = poisonInfo;
    this.receiver = receiver;
    base.caster = caster;
    Icon = effectIcon;
    dmgTimer = poisonInfo.DmgDelay;
    Duration = poisonInfo.Duration;
    Timer = Duration;
        
    damageInfo.AddDamage(this.poisonInfo.Damage, multiplier: caster.Params.GetDamageAmplification(DamageType.Physic));

    currentVisualPrefab = Instantiate(visual, transform.position, Quaternion.identity, transform);
}
public void Update()
{
    dmgTimer -= Time.deltaTime;
    Timer -= Time.deltaTime;
    if (dmgTimer <= 0)
    {
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
