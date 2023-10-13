using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseAttackVisualSpawner : MonoBehaviour
{
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private Unit unit;
    void Awake()
    {
        unit.OnBaseAttack += SpawnVisual;
    }

    private void SpawnVisual(float dir, float angle, float dist)
    {
        BaseAttackVisual attackVisual = Instantiate(visualPrefab, transform.position, Quaternion.identity)
            .GetComponent<BaseAttackVisual>();
        attackVisual.Swing(dir, angle, dist);
    }
}
