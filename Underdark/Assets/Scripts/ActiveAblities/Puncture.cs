using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puncture : ActiveAbility, IAttackerTarget
{
    public Transform Transform => transform;
    
    [SerializeField] private ScalableProperty<int> attacksCount;
    [SerializeField] private float attackDelay;

    [Header("Visual")] 
    [SerializeField] private PunctureVisual visualPrefab;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, base.attackDir);

        int damage = (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            MaxValue.GetValue(abilityLevel));
        damageInfo.AddDamage(damage, multiplier: caster.Params.GetDamageAmplification(damageType));
        
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        for (int i = 0; i < attacksCount.GetValue(abilityLevel); i++)
        {
            var target = FindClosestTarget(caster);

            var visualPos = target == null
                ? (Vector3)attackDir + transform.position
                : target.transform.position;
            var newVisual = Instantiate(visualPrefab, visualPos, Quaternion.identity, transform);
            newVisual.StartVisualEffect(visualPos);
        
            if (target != null) 
                Attack(target.GetComponent<IDamageable>());
            
            yield return new WaitForSeconds(attackDelay);
        }
    }

    public void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, this, damageInfo))
        {
            foreach (var debuffInfo in debuffInfos.GetValue(abilityLevel).DebuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
}
