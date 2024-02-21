using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : ActiveAbility, IAttackerTarget
{
    public Transform Transform { get; }
    
    [SerializeField] private ActiveAbilitySO subAbilitySO;
    [SerializeField] private ScalableProperty<int> levelsOfSubAbility;
    [SerializeField] private ScalableProperty<int> chargeCount;
    [SerializeField] private ScalableProperty<float> lifeTime;
    [SerializeField] private float attackDelay;

    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        Destroy(gameObject, lifeTime.GetValue(abilityLevel));

        StartCoroutine(TryToAttack());
    }

    IEnumerator TryToAttack()
    {
        var attackLeft = chargeCount.GetValue(abilityLevel);
        
        while (true)
        {
            yield return new WaitForSeconds(attackDelay);
            
            var target = FindClosestTarget(base.caster, transform.position, AttackDistance.GetValue(abilityLevel));
            if (target != null && target.TryGetComponent(out IDamageable damageable))
            {
                Attack(damageable);
                attackLeft--;
            }
            
            if (attackLeft <= 0)
                Destroy(gameObject);
        }
    }
    
    
    public void Attack(IDamageable damageable)
    {
        var newAbility = Instantiate(subAbilitySO.ActiveAbility, transform.position, Quaternion.identity);
        newAbility.Execute(caster, levelsOfSubAbility.GetValue(abilityLevel),
            damageable.Transform.position - transform.position, mustAggro: false);
    }
}
