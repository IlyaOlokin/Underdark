using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongSmash : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    
    [Header("Visual")]
    [SerializeField] private RadialFillVisual visual;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    [SerializeField] private GameObject hitVisualPref;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        
        InitDamage(caster);
        Attack();
        
        StartCoroutine(visual.StartVisual(AttackDistance.GetValue(abilityLevel), caster.GetAttackDirAngle(attackDir),
            AttackAngle.GetValue(abilityLevel), visualDuration, scaleLerpSpeed));
    }
    
    public void Attack()
    {
        var targets = FindAllTargets(caster, caster.transform.position, AttackDistance.GetValue(abilityLevel));

        foreach (var target in targets)
        {
            if (target.GetComponent<IDamageable>().TakeDamage(caster, this, damageInfo))
            {
                foreach (var debuffInfo in debuffInfos.GetValue(abilityLevel).DebuffInfos)
                {
                    debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
                }
            }

            Instantiate(hitVisualPref, target.transform.position, Quaternion.identity);
        }
    }
}
