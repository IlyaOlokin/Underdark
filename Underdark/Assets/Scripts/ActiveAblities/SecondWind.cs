using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SecondWind : ActiveAbility
{
    [SerializeField] private BaseStat secondStat;
    [SerializeField] private float effectDuration;
    [SerializeField] private ScalableProperty<PassivesList> passives;
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);

        var healAmount = Mathf.Max(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            caster.Stats.GetTotalStatValue(secondStat) * StatMultiplier.GetValue(abilityLevel));
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);

        var currentPassive = passives.GetValue(abilityLevel);
        foreach (var passive in currentPassive.Passives)
        {
            Buff.ApplyBuff(base.caster, passive, effectDuration);
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[6];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        res[1] = $"Heal: {StatMultiplier.GetValue(currentLevel)} * max({UnitStats.GetStatString(baseStat)}, {UnitStats.GetStatString(secondStat)})";
        if (manaCost.GetValue(currentLevel) != 0)    res[2] = $"Mana: {manaCost.GetValue(currentLevel)}";
        res[3] = $"Duration: {effectDuration}";
        if (Cooldown.GetValue(currentLevel) != 0)    res[5] = $"Cooldown: {Cooldown.GetValue(currentLevel)}";
        
        return res;
    }
    
    public override string[] ToStringAdditional(Unit owner)
    {
        List<string> res = new List<string>();
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        for (int i = 0; i < passives.GetValue(currentLevel).Passives.Count; i++)
        {
            res.Add(passives.GetValue(currentLevel).Passives[i].ToString());
        }

        return res.ToArray();
    }
}
