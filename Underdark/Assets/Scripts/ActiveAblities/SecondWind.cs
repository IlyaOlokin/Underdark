using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecondWind : ActiveAbility
{
    [SerializeField] private BaseStat secondStat;
    [SerializeField] private float damageAmplification;
    [SerializeField] private float damageAmplificationDuration;
    [SerializeField] private Sprite buffIcon;
    [SerializeField] private PassiveSO passive;
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, attackDir);

        var healAmount = Mathf.Max(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            caster.Stats.GetTotalStatValue(secondStat) * StatMultiplier.GetValue(abilityLevel));
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);
        
        Buff.ApplyBuff(base.caster, passive, buffIcon, damageAmplificationDuration);
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[3];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));
        
        res[0] = description;
        res[1] = $"Heal: {StatMultiplier.GetValue(currentLevel)} * max({UnitStats.GetStatString(baseStat)}, {UnitStats.GetStatString(secondStat)})";
        res[2] = $"Damage Amplification: {damageAmplification * 100}% ";
        return res;
    }
}
