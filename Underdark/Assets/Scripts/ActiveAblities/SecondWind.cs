using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecondWind : ActiveAbility
{
    [SerializeField] private BaseStat secondStat;
    [SerializeField] private float damageAmplificationDuration;
    [SerializeField] private ScalableProperty<PassivesList> passives;
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, attackDir);

        var healAmount = Mathf.Max(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            caster.Stats.GetTotalStatValue(secondStat) * StatMultiplier.GetValue(abilityLevel));
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);

        var currentPassive = passives.GetValue(abilityLevel);
        foreach (var passive in currentPassive.Passives)
        {
            Buff.ApplyBuff(base.caster, passive, damageAmplificationDuration);
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }

    public override string[] ToString(Unit owner)
    {
        var res = new List<string>(4);
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        res.Add(description);
        res.Add($"Heal: {StatMultiplier.GetValue(currentLevel)} * max({UnitStats.GetStatString(baseStat)}, {UnitStats.GetStatString(secondStat)})");
        for (int i = 0; i < passives.GetValue(currentLevel).Passives.Count; i++)
        {
            res.Add(passives.GetValue(currentLevel).Passives[i].ToString());
        }
        return res.ToArray();
    }
}
