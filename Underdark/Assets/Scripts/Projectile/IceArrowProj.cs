using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceArrowProj : Projectile
{
    [SerializeField] private ActiveAbilitySO subAbilitySO;
    [SerializeField] private ScalableProperty<int> levelsOfSubAbility;
    [SerializeField] private ScalableProperty<ProjectileShotInfo> projectileShotInfos;
    [SerializeField] private float angleOffset;
    [SerializeField] private float spawnPosOffset;
    protected override void Die(IDamageable damageable)
    {
        var newLevel = levelsOfSubAbility.GetValue(abilityLevel);
        
        if (newLevel >= 1)
            SpawnSubAbility(damageable, newLevel);
        
        coll.enabled = false;
        rb.velocity = Vector2.zero; // SpawnVisual
        Destroy(gameObject);
    }

    private void SpawnSubAbility(IDamageable damageable, int newLevel)
    {
        var damageablestoIgnore = new List<IDamageable>() { damageable };
        var spawnPos = damageable == null ? transform.position : damageable.Transform.position;
        var projectileShotInfo = projectileShotInfos.GetValue(abilityLevel);
        var angle = angleOffset + Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(Vector2.right, rb.velocity.normalized));
        if (rb.velocity.y < 0) angle *= -1;

        for (int i = 0; i < projectileShotInfo.ProjCountInShot; i++)
        {
            var localAngle = angle + projectileShotInfo.AngleBetweenProj * ((i + 1) / 2) * (i % 2 == 0 ? 1 : -1);
            var localDir = new Vector3(Mathf.Cos(localAngle * Mathf.Deg2Rad), Mathf.Sin(localAngle * Mathf.Deg2Rad));

            var newAbility = Instantiate(subAbilitySO.ActiveAbility, spawnPos + localDir.normalized * spawnPosOffset,
                Quaternion.identity);
            newAbility.Execute(caster, newLevel, localDir, damageablestoIgnore);
        }
    }
}
