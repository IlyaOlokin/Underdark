using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        damageInfo.AddDamage(
            (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
                MaxValue.GetValue(abilityLevel)), damageType, caster.Params.GetDamageAmplification(damageType));

        StartCoroutine(InstantiateProjectiles());
    }

    protected virtual IEnumerator InstantiateProjectiles()
    {
        var currShotInfo = shotInfo.GetValue(abilityLevel);
        var currProjPref = projectilePref.GetValue(abilityLevel);
        var destroyDelay = AttackDistance.GetValue(abilityLevel) / projSpeed;
        var penetrations = this.penetrationCount.GetValue(abilityLevel);
        var angle = Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, attackDir));
        if (attackDir.y < 0) angle *= -1;
        
        for (int i = 0; i < currShotInfo.Shots; i++)
        {
            for (int j = 0; j < currShotInfo.ProjCountInShot; j++)
            {
                var localAngle = angle + currShotInfo.AngleBetweenProj * ((j + 1) / 2) * (j % 2 == 0 ? 1 : -1);
                var localDir = new Vector2(Mathf.Cos(localAngle * Mathf.Deg2Rad), Mathf.Sin(localAngle * Mathf.Deg2Rad));
                var velocity = localDir * projSpeed; 
                
                var newProj = Instantiate(currProjPref, transform.position, Quaternion.identity);
                newProj.Init(caster, damageInfo, debuffInfos.GetValue(abilityLevel).DebuffInfos, abilityLevel, velocity,
                    destroyDelay, penetrations, damageablesToIgnore);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && distToTarget > 2;
    }
}
