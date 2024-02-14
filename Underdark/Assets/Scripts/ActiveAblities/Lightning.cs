using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Lightning : ActiveAbility, IAttackerTarget
{
    public Transform Transform => transform;

    [SerializeField] private ScalableProperty<int> lightningCount;
    
    [Header("Visual")] 
    [SerializeField] private GameObject lightningPref;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore = null)
    { 
        base.Execute(caster, level, attackDir);
        
        int damage = (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
            MaxValue.GetValue(abilityLevel));
        damageInfo.AddDamage(damage, multiplier: caster.Params.GetDamageAmplification(damageType));

        StartCoroutine(ShootLightning(caster.transform.position, 1, lightningCount.GetValue(abilityLevel),
            new List<IDamageable>()));
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
    
    private IEnumerator ShootLightning(Vector3 startPos, float dmgMultiplier, int lightningsLeft, List<IDamageable> pickedEnemies)
    {
        var target = FindClosestTarget(caster, startPos);
        
        if (lightningsLeft == 0 || target == null || !target.TryGetComponent(out IDamageable damageable))
        {
            Destroy(gameObject);
            yield break;
        }
        
        var endPos = damageable.Transform.position;

        var newLightning = Instantiate(lightningPref, transform.position, Quaternion.identity);
        var lineRenderer = newLightning.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        
        Attack(damageable);
        pickedEnemies.Add(damageable);

        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(ShootLightning(endPos, dmgMultiplier, lightningsLeft--, pickedEnemies));
    }
}
