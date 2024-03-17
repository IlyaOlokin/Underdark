using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CircularSwing : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    [SerializeField] private ScalableProperty<int> attacksCount;
    [SerializeField] private ScalableProperty<float> attackDelay;
    [SerializeField] private PassiveSO slowMovement;
    [SerializeField] private float pullDistance;
    [SerializeField] private DebuffInfo pullEffect;
    
    [Header("Visual")]
    [SerializeField] private BaseAttackVisual visualPrefab;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        
        transform.SetParent(caster.transform);
        InitDamage(caster);
        
        Buff.ApplyBuff(caster, slowMovement, attackDelay.GetValue(abilityLevel) * attacksCount.GetValue(abilityLevel));

        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        var attackDirAngle = Vector3.Angle(Vector3.right, attackDir);
        
        for (int i = 0; i < attacksCount.GetValue(abilityLevel); i++)
        {
            Attack();

            var newVisual = Instantiate(visualPrefab, transform.position, Quaternion.identity);
            newVisual.transform.SetParent(caster.transform);
            newVisual.Swing(attackDirAngle, AttackAngle.GetValue(abilityLevel), AttackDistance.GetValue(abilityLevel), true, attackDelay.GetValue(abilityLevel) + 0.1f);

            if (abilityLevel == 5 && i == 2)
                PullTargets();
            
            yield return new WaitForSeconds(attackDelay.GetValue(abilityLevel));
        }
        
        Destroy(gameObject);
    }

    private void PullTargets()
    {
        var targets = FindAllTargets(caster, caster.transform.position, pullDistance, AttackAngle.GetValue(abilityLevel));

        foreach (var target in targets)
        {
            if (target.TryGetComponent(out Unit unit))
            {
                pullEffect.Execute(this, unit, caster);
            }
        }
    }
    
    public void Attack()
    {
        var targets = FindAllTargets(caster, caster.transform.position, AttackDistance.GetValue(abilityLevel), AttackAngle.GetValue(abilityLevel));

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
