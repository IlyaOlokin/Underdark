using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Lightning : ActiveAbility, IAttackerTarget
{
    public Transform Transform => transform;

    [SerializeField] private ScalableProperty<int> lightningCount;
    [SerializeField] private ScalableProperty<float> lightningBounceDist;
    [SerializeField] private ScalableProperty<float> bounceDamageMultiplier;
    
    [Header("Visual")] 
    [SerializeField] private GameObject lightningPref;
    [SerializeField] private GameObject sparksPref;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore = null,bool mustAggro = true)
    { 
        base.Execute(caster, level, attackDir);
        
        InitDamage(caster);

        StartCoroutine(ShootLightning(caster.transform.position, 1, lightningCount.GetValue(abilityLevel),
            AttackDistance.GetValue(abilityLevel),
            new List<IDamageable>()));
    }

    private IEnumerator ShootLightning(Vector3 startPos, float dmgMultiplier, int lightningsLeft, float bounceDist, List<IDamageable> pickedTargets)
    {
        var target = FindClosestTarget(caster, startPos, bounceDist, pickedTargets);
        
        if (lightningsLeft == 0 || target == null || !target.TryGetComponent(out IDamageable damageable))
        {
            Destroy(gameObject);
            yield break;
        }
        
        var endPos = damageable.Transform.position;

        var newLightning = Instantiate(lightningPref, startPos, Quaternion.identity);
        SetVisuals(newLightning, startPos, endPos);

        Attack(damageable);
        pickedTargets.Add(damageable);

        yield return new WaitForSeconds(0.1f);
        InitDamage(caster, bounceDamageMultiplier.GetValue(abilityLevel));
        
        StartCoroutine(ShootLightning(endPos, dmgMultiplier, --lightningsLeft, lightningBounceDist.GetValue(abilityLevel), pickedTargets));
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
    
    private void SetVisuals(GameObject newLightning, Vector3 startPos,  Vector3 endPos)
    {
        var lineRenderer = newLightning.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        Instantiate(sparksPref, endPos, quaternion.identity);

        var perpendicular = Vector3.Cross(endPos - startPos, Vector3.forward).normalized;

        newLightning.GetComponent<Light2D>().SetShapePath(new[]
        {
            perpendicular * 0.1f,
            endPos - startPos + perpendicular * 0.1f,
            endPos - startPos - perpendicular * 0.1f,
            -perpendicular * 0.1f
        });
    }
}
