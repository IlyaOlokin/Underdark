using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSwing : ActiveAbility, IAttacker
{
    public Transform Transform => transform;
    [Header("Visual")]
    [SerializeField] private BaseAttackVisual visual;
    
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);

        this.caster = caster;
        damage = Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        
        Attack();
        
        var attackDirAngle = Vector3.Angle(Vector3.right, caster.GetAttackDirection());
        visual.Swing(attackDirAngle, AttackAngle, AttackDistance);
    }
    
    public void Attack()
    {
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            if (target.GetComponent<IDamageable>().TakeDamage(caster, this, damage))
            {
                foreach (var debuffInfo in debuffInfos)
                {
                    debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
                }
            }
        }
    }

    public void Attack(IDamageable unit)
    {
        throw new System.NotImplementedException();
    }
}
