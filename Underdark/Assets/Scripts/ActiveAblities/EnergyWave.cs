using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnergyWave : ActiveAbility
{
    [Header("Visual")]
    [SerializeField] private RadialFillVisual visual;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, attackDir);
        
        var targets = FindAllTargets(caster, caster.transform.position, AttackDistance.GetValue(abilityLevel));

        foreach (var target in targets)
        {
            foreach (var debuffInfo in debuffInfos.GetValue(abilityLevel).DebuffInfos)
            {
                debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
            }
        }

        
        StartCoroutine(visual.StartVisual(AttackDistance.GetValue(abilityLevel), caster.GetAttackDirAngle(attackDir),
            AttackAngle.GetValue(abilityLevel), visualDuration, scaleLerpSpeed));
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && distToTarget < 2;
    }
}
