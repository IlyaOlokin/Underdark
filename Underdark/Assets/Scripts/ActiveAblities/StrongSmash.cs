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
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.GetStatValue(baseStat) * statMultiplier, maxValue);

        OverrideWeaponStats(caster.GetWeapon());
        Attack();
        
        StartCoroutine(StartVisual());
    }
    
    IEnumerator StartVisual()
    {
        visualSR.material = new Material(visualSR.material);
        
        var targetScale = transform.localScale * (attackDistance * 2 + 1);
        transform.localScale = Vector3.zero;
        var attackDir = Vector2.Angle(Vector2.right, caster.GetAttackDirection());
        if (caster.GetAttackDirection().y < 0) attackDir *= -1;
        visualSR.material.SetFloat("_Turn", attackDir);
        visualSR.material.SetFloat("_FillAmount", attackAngle);
        
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
            if (target.GetComponent<IDamageable>().TakeDamage(caster, damage))
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
