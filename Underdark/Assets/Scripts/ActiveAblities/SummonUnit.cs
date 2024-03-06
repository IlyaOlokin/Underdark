using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SummonUnit : ActiveAbility
{
    [SerializeField] private ScalableProperty<SummonList> summonPref;
    [SerializeField] private float spawnRadius = 2;
    [SerializeField] private float spawnDelay = 0.1f;
    [SerializeField] private float rotateSpawnPointsDelay = 10f;

    [Header("Visual")] 
    [SerializeField] private GameObject summonVisual;

    private List<Transform> parents = new List<Transform>();
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);

        caster.OnUnitDeath += UnParent;
        transform.SetParent(caster.transform);
        
        CreateParents();

        StartCoroutine(SpawnAllSummons());
        StartCoroutine(RotateSpawnPoints());
    }

    private IEnumerator SpawnAllSummons()
    {
        var summons = summonPref.GetValue(abilityLevel).Summons;
        for (var i = 0; i < summons.Count; i++)
        {
            SpawnSummon(summons[i].npcUnit, parents[i]);
            Instantiate(summonVisual, parents[i].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnSummon(NPCUnit summon, Transform parent)
    {
        var npc = Instantiate(summon, parent.transform.position, Quaternion.identity, transform);
        npc.SetSummonedUnit(parent, caster.transform.tag, caster.gameObject.layer, caster.AttackMask, caster.AlliesLayer);
    }

    private IEnumerator RotateSpawnPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(rotateSpawnPointsDelay);
            foreach (var parent in parents)
            {
                parent.transform.localPosition = GetRandomPos();
            }
        }
    }

    private void CreateParents()
    {
        for (int i = 0; i < summonPref.GetValue(abilityLevel).Summons.Count; i++)
        {
            var parent = new GameObject("SummonParent");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = GetRandomPos();
            parents.Add(parent.transform);
        }
    }

    private Vector2 GetRandomPos()
    {
        var localAngle = Random.Range(0f, 360f);
        var dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * localAngle), Mathf.Sin(Mathf.Deg2Rad * localAngle));
        return dir.normalized * spawnRadius ;
    }

    private void UnParent()
    {
        transform.SetParent(null);
    }

    private void OnDisable()
    {
        caster.OnUnitDeath -= UnParent;
    }
}
