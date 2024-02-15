using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSwing : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    [Header("Visual")]
    [SerializeField] private BaseAttackVisual visual;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, attackDir);
        
        InitDamage(caster);
        
        Attack();
        
        var attackDirAngle = Vector3.Angle(Vector3.right, base.attackDir);
        visual.Swing(attackDirAngle, AttackAngle.GetValue(abilityLevel), AttackDistance.GetValue(abilityLevel), true);
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
        }
    }
}
