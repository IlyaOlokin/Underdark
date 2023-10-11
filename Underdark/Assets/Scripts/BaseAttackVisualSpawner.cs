using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackVisualSpawner : MonoBehaviour
{
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private Player player;
    void Awake()
    {
        player.OnBaseAttack += SpawnVisual;
    }

    private void SpawnVisual(float dir, float angle, float dist)
    {
        BaseAttackVisual attackVisual = Instantiate(visualPrefab, transform.position, Quaternion.identity)
            .GetComponent<BaseAttackVisual>();
        attackVisual.Swing(dir, angle, dist);
    }
}
