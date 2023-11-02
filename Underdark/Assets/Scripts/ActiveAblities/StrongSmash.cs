using System.Collections;
using UnityEngine;

public class StrongSmash : ActiveAblity
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer visualSR;
    [SerializeField] private float visualDuration;
    
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.Strength * statMultiplier, maxValue);

        OverrideWeaponStats(caster.GetWeapon());
        Attack();
        
        StartCoroutine(StartVisual());
    }

    IEnumerator StartVisual()
    {
        visualSR.material = new Material(visualSR.material);

        transform.localScale = Vector3.zero;
        float scaleSpeed = (attackDistance * 2 + 0.77f) / visualDuration;
        var attackDir = Vector2.Angle(Vector2.right, caster.GetAttackDirection());
        if (caster.GetAttackDirection().y < 0) attackDir *= -1;
        visualSR.material.SetFloat("_Turn", attackDir);
        visualSR.material.SetFloat("_FillAmount", attackAngle);

        while (visualDuration > 0)
        {
            visualDuration -= Time.deltaTime;
            transform.localScale += new Vector3(scaleSpeed * Time.deltaTime, scaleSpeed * Time.deltaTime);
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
