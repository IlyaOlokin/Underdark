using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ProjectileAbility : ActiveAbility
{
    [Header("Projectile Settings")] 
    [SerializeField] private ScalableProperty<Projectile> projectilePref;
    [SerializeField] private DistributionType distributionType;
    [SerializeField] protected ScalableProperty<ProjectileShotInfo> shotInfo;
    [SerializeField] protected ScalableProperty<bool> ableToRicochet;
    [SerializeField] protected ScalableProperty<int> penetrationCount;
    [SerializeField] protected float projSpeed;

    public override void Execute(Unit caster, int exp, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore = null,bool mustAggro = true)
    {
        base.Execute(caster, exp, attackDir, damageablesToIgnore);
        transform.parent = caster.transform;
        
        InitDamage(caster);

        switch (distributionType)
        {
            case DistributionType.Exact:
                StartCoroutine(InstantiateProjectilesExactDistribution());
                break;
            case DistributionType.Triangular:
                StartCoroutine(InstantiateProjectilesTriangularDistribution());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator InstantiateProjectilesExactDistribution()
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
    
    private IEnumerator InstantiateProjectilesTriangularDistribution()
    {
        var currShotInfo = shotInfo.GetValue(abilityLevel);
        var meanAngle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, attackDir));
        if (attackDir.y < 0) meanAngle *= -1;
        Random rand = new Random();

        for (int i = 0; i < currShotInfo.Shots; i++)
        {
            for (int j = 0; j < currShotInfo.ProjCountInShot; j++)
            {
                var localAngle = meanAngle;
                if (j != 0 || i != 0)
                {
                    localAngle = NextTriangular(rand, meanAngle - currShotInfo.AngleBetweenProj,
                        meanAngle + currShotInfo.AngleBetweenProj, meanAngle);
                }

                var localDir = new Vector2(Mathf.Cos(localAngle * Mathf.Deg2Rad),
                    Mathf.Sin(localAngle * Mathf.Deg2Rad));
                var velocity = localDir * projSpeed;

                SpawnProjectile(velocity);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnProjectile(Vector2 velocity)
    {
        var newProj = Instantiate(projectilePref.GetValue(abilityLevel), transform.position, Quaternion.identity);
        newProj.Init(caster, damageInfo, debuffInfos.GetValue(abilityLevel).DebuffInfos, abilityLevel, velocity,
            AttackDistance.GetValue(abilityLevel) / projSpeed, penetrationCount.GetValue(abilityLevel),
            ableToRicochet.GetValue(abilityLevel), damageablesToIgnore);
    }
    
    private float NextTriangular(Random rand, double min, double max, double mean)
    {
        var u = rand.NextDouble();
        
        var res = u < (mean - min) / (max - min)
            ? min + Math.Sqrt(u * (max - min) * (mean - min))
            : max - Math.Sqrt((1 - u) * (max - min) * (max - mean));

        return (float) res;
    }
}
