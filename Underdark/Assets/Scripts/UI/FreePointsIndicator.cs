using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePointsIndicator : MonoBehaviour
{
    [SerializeField] private GameObject freePointsIndicator;

    private UnitStats stats;
    public void Init(UnitStats stats)
    {
        this.stats = stats;
        stats.OnLevelUp += UpdateVisual;
        stats.OnStatsChanged += UpdateVisual;
        UpdateVisual();
    }

    
    void UpdateVisual()
    {
        freePointsIndicator.SetActive(stats.FreePoints > 0);
    }
}
