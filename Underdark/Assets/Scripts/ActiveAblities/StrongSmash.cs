using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StrongSmash : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;

    [Header("Shock Wave Setup")] 
    [SerializeField] private ScalableProperty<float> attackDistanceShockWave;
    [SerializeField] private ScalableProperty<float> maxValueShockWave;
    [SerializeField] private ScalableProperty<float> statMultiplierShockWave;
    [SerializeField] private ScalableProperty<DebuffInfoList> debuffInfosShockWave;
    [SerializeField] private float shockWaveMinLevel;

    private DamageInfo shockWaveDamage;
    
    [Header("Visual")]
    [SerializeField] private RadialFillVisual visual;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    [SerializeField] private GameObject hitVisualPref;

    [SerializeField] private ShockWaveVisual shockWaveVisualPref;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        
        InitDamage(caster);
        if (abilityLevel >= shockWaveMinLevel)
            shockWaveDamage = InitDamageLocal(caster, baseStat, statMultiplierShockWave.GetValue(abilityLevel),
                maxValueShockWave.GetValue(abilityLevel), damageType);
        Attack();
        
        StartVisual();
    }

    private void StartVisual()
    {
        StartCoroutine(visual.StartVisual(AttackDistance.GetValue(abilityLevel), caster.GetAttackDirAngle(attackDir),
            AttackAngle.GetValue(abilityLevel), visualDuration, scaleLerpSpeed));

        if (abilityLevel >= shockWaveMinLevel)
        {
            var shockWave = Instantiate(shockWaveVisualPref, transform.position, Quaternion.identity, transform);
            shockWave.StartVisual(visualDuration, attackDistanceShockWave.GetValue(abilityLevel));
        }
    }

    public void Attack()
    {
        var targets = FindAllTargets(caster, caster.transform.position, AttackDistance.GetValue(abilityLevel), AttackAngle.GetValue(abilityLevel));

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

        if (abilityLevel >= shockWaveMinLevel)
        {
            var targetsShockWave = FindAllTargets(caster, caster.transform.position, attackDistanceShockWave.GetValue(abilityLevel), 360f);

            foreach (var target in targetsShockWave)
            {
                if (target.GetComponent<IDamageable>().TakeDamage(caster, this, shockWaveDamage))
                {
                    foreach (var debuffInfo in debuffInfosShockWave.GetValue(abilityLevel).DebuffInfos)
                    {
                        debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
                    }
                }

                Instantiate(hitVisualPref, target.transform.position, Quaternion.identity);
            }
        }
    }
    
    public override string[] ToStringAdditional(Unit owner)
    {
        List<string> res = base.ToStringAdditional(owner).ToList();
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        foreach (var debuffInfo in debuffInfosShockWave.GetValue(currentLevel).DebuffInfos)
        {
            res.Add($"Shock wave: {debuffInfo.ToString()}");
        }

        return res.ToArray();
    }
}
