using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonUnit : ActiveAbility
{
    [SerializeField] private ScalableProperty<EnemySpawnPoint> summonPref;
    [SerializeField] private float spawnRadius = 1;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        
        transform.SetParent(caster.transform);
        transform.position += Vector3.right * spawnRadius;
        var npc = Instantiate(summonPref.GetValue(abilityLevel).npcUnit, transform.position + Vector3.right, Quaternion.identity, transform);
        npc.SetSummonedUnit(transform, caster.transform.tag, caster.gameObject.layer, caster.AttackMask, caster.AlliesLayer);
    }
}
