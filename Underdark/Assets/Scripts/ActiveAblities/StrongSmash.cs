using System.Collections;
using UnityEngine;

public class StrongSmash : ActiveAbility, IAttacker
{
    public Transform Transform => transform;
    
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSR;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        int damage = (int) Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        damageInfo.AddDamage(damage);
        Attack();
        
        StartCoroutine(StartVisual());
    }
    
    IEnumerator StartVisual()
    {
        visualSR.material = new Material(visualSR.material);
        
        var targetScale = transform.localScale * (AttackDistance * 2 + 1);
        transform.localScale = Vector3.zero;
        
        visualSR.material.SetFloat("_Turn", caster.GetAttackDirAngle());
        visualSR.material.SetFloat("_FillAmount", AttackAngle);
        
        while (visualDuration > 0)
        {
            transform.localScale = Vector3.Lerp( transform.localScale, targetScale, scaleLerpSpeed / visualDuration * Time.deltaTime);
            visualDuration -= Time.deltaTime;
            yield return null;
        }
    }
    
    public void Attack()
    {
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            if (target.GetComponent<IDamageable>().TakeDamage(caster, this, damageInfo))
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
