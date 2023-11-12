using System.Collections;
using UnityEngine;

public class Puncture : ActiveAbility, IAttacker
{
    public Transform Transform => transform;
    
    [SerializeField] private int targetsCount;
    [SerializeField] private float attackDelay;

    [Header("Visual")] 
    [SerializeField] private PunctureVisual visualPrefab;
    
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        damage = Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);

        for (int i = 0; i < targetsCount; i++)
        {
            var target = FindClosestTarget(caster);
        
            if (target == null) continue;
            Attack(target.GetComponent<IDamageable>());
            
            var newVisual = Instantiate(visualPrefab, target.transform.position, Quaternion.identity, transform);
            newVisual.StartVisualEffect(target.transform);
            
            yield return new WaitForSeconds(attackDelay);
        }
    }
    
    public void Attack()
    {
        
    }

    public void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, this, damage))
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
}
