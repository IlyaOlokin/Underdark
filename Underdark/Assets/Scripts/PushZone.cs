using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushZone : MonoBehaviour, IAttackerTarget
{
    public Transform Transform => transform;

    private Unit caster;
    private DamageInfo damageInfo;
    private List<DebuffInfo> debuffInfos;
    private int abilityLevel;

    public void Init(Unit caster, DamageInfo damageInfo, List<DebuffInfo> debuffInfos, int abilityLevel)
    {
        this.caster = caster;
        this.damageInfo = damageInfo;
        this.debuffInfos = debuffInfos;
        this.abilityLevel = abilityLevel;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (caster.AttackMask == (caster.AttackMask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out Unit unit))
            {
                Attack(unit);
                foreach (var debuffInfo in debuffInfos)
                {
                    debuffInfo.Execute(this, unit, caster);
                }
            }
        }
    }
    
    public void Attack(IDamageable damageable)
    {
        if (abilityLevel <= 1) return;
        damageable.TakeDamage(caster, this, damageInfo);
    }
}
