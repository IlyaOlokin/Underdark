using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWave : ActiveAbility
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSR;
    [SerializeField] private float visualDuration;
    [SerializeField] private float scaleLerpSpeed;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);

        this.caster = caster;
        
        var targets = FindAllTargets(caster);

        foreach (var target in targets)
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(caster, target.GetComponent<Unit>(), caster);
            }
        }
        
        StartCoroutine(StartVisual());
    }
    
    IEnumerator StartVisual()
    {
        visualSR.material = new Material(visualSR.material);
        
        var targetScale = transform.localScale * (AttackDistance * 2 + 1);
        transform.localScale = Vector3.zero;
        
        var attackDir = Vector2.Angle(Vector2.right, caster.GetAttackDirection());
        if (caster.GetAttackDirection().y < 0) attackDir *= -1;
        visualSR.material.SetFloat("_Turn", attackDir);
        visualSR.material.SetFloat("_FillAmount", AttackAngle);
        
        while (visualDuration > 0)
        {
            transform.localScale = Vector3.Lerp( transform.localScale, targetScale, scaleLerpSpeed / visualDuration * Time.deltaTime);
            visualDuration -= Time.deltaTime;
            yield return null;
        }
    }
}
