using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratedRegeneration : ActiveAbility
{
    [SerializeField] private ScalableProperty<float> effectDuration;
    [SerializeField] private ScalableProperty<PassivesList> passives;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        
        transform.SetParent(caster.transform);

        var currentPassive = passives.GetValue(abilityLevel);
        foreach (var passive in currentPassive.Passives)
        {
            Buff.ApplyBuff(base.caster, passive, effectDuration.GetValue(abilityLevel));
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
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
