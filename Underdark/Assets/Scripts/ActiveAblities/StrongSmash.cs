using System.Collections;
using UnityEngine;

public class StrongSmash : ActiveAbility, IAttackerAOE
{
    public Transform Transform => transform;
    
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSR;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    
    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);
        int damage = (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            MaxValue.GetValue(abilityLevel));
        damageInfo.AddDamage(damage, multiplier: caster.Params.GetDamageAmplification(damageType));
        Attack();
        
        StartCoroutine(StartVisual());
    }
    
    IEnumerator StartVisual()
    {
        visualSR.material = new Material(visualSR.material);
        
        var targetScale = transform.localScale * (AttackDistance.GetValue(abilityLevel) * 2 + 1);
        transform.localScale = Vector3.zero;
        
        visualSR.material.SetFloat("_Turn", caster.GetAttackDirAngle(attackDir));
        visualSR.material.SetFloat("_FillAmount", AttackRadius.GetValue(abilityLevel));
        
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
                foreach (var debuffInfo in debuffInfos.GetValue(abilityLevel).DebuffInfos)
                {
                    debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
                }
            }
        }
    }
}
