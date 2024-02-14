using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ProjectileAbility : ActiveAbility
{
    [Header("Projectile Settings")] 
    [SerializeField] private ScalableProperty<Projectile> projectilePref;
    [SerializeField] protected ScalableProperty<ProjectileShotInfo> shotInfo;
    [SerializeField] protected ScalableProperty<int> penetrationCount;
    [SerializeField] protected float projSpeed;

    public override void Execute(Unit caster, int exp, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore = null)
    {
        base.Execute(caster, exp, attackDir, damageablesToIgnore);
        transform.parent = caster.transform;

        damageInfo.AddDamage(
            (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
                MaxValue.GetValue(abilityLevel)), damageType, caster.Params.GetDamageAmplification(damageType));

        StartCoroutine(InstantiateProjectiles());
    }

    protected virtual IEnumerator InstantiateProjectiles()
    {
        var currShotInfo = shotInfo.GetValue(abilityLevel);
        var angle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, attackDir));
        if (attackDir.y < 0) angle *= -1;
        
        for (int i = 0; i < currShotInfo.Shots; i++)
        {
            for (int j = 0; j < currShotInfo.ProjCountInShot; j++)
            {
                var localAngle = angle + currShotInfo.AngleBetweenProj * ((j + 1) / 2) * (j % 2 == 0 ? 1 : -1);
                var localDir = new Vector2(Mathf.Cos(localAngle * Mathf.Deg2Rad), Mathf.Sin(localAngle * Mathf.Deg2Rad));
                var velocity = localDir * projSpeed; 
                
                SpawnProjectile(velocity);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected void SpawnProjectile(Vector2 velocity)
    {
        var newProj = Instantiate(projectilePref.GetValue(abilityLevel), transform.position, Quaternion.identity);
        newProj.Init(caster, damageInfo, debuffInfos.GetValue(abilityLevel).DebuffInfos, abilityLevel, velocity,
            AttackDistance.GetValue(abilityLevel) / projSpeed, penetrationCount.GetValue(abilityLevel), damageablesToIgnore);
    }

    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && distToTarget > 2;
    }
    
    protected float NextTriangular(Random rand, double min, double max, double mean)
    {
        var u = rand.NextDouble();
        
        var res = u < (mean - min) / (max - min)
            ? min + Math.Sqrt(u * (max - min) * (mean - min))
            : max - Math.Sqrt((1 - u) * (max - min) * (max - mean));

        return (float) res;
    }
}
