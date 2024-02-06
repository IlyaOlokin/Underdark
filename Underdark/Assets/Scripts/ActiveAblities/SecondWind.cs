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
    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);

        var healAmount = Mathf.Max(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, caster.Stats.GetTotalStatValue(secondStat) * statMultiplier);
        transform.SetParent(caster.transform);
        caster.RestoreHP(healAmount, true);

        var newBuff = caster.AddComponent<AllDamageAmplification>();
        newBuff.Init(damageAmplificationDuration, damageAmplification, caster, buffIcon);
        caster.ReceiveStatusEffect(newBuff);
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }

    public override string[] ToString(Unit owner)
    {
        var res = new string[3];
        res[0] = description;
        res[1] = $"Heal: {statMultiplier} * max({UnitStats.GetStatString(baseStat)}, {UnitStats.GetStatString(secondStat)})";
        res[2] = $"Damage Amplification: {damageAmplification * 100}% ";
        return res;
    }
}
