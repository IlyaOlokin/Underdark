using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BonusStatElixir : Elixir
{
    [SerializeField] private BaseStat bonusStat;
    [SerializeField] private int bonusValue;
    [SerializeField] private float durationInSeconds;
    private Unit localCaster;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<BonusStatElixir>();
        comp.Init(bonusStat, bonusValue, durationInSeconds, caster);
    }

    public void Init(BaseStat bonusStat, int bonusValue, float duration, Unit caster)
    {
        this.bonusStat = bonusStat;
        this.bonusValue = bonusValue;
        durationInSeconds = duration;
        localCaster = caster;
        localCaster.Stats.ApplyBonusStat(bonusStat, bonusValue);
    }

    private void Update()
    {
        durationInSeconds -= Time.deltaTime;
        if (durationInSeconds <= 0)
            Destroy(this);
    }

    private void OnDestroy()
    {
        localCaster.Stats.ApplyBonusStat(bonusStat, -bonusValue);
    }


    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        
        res[0] = string.Format(description, UnitStats.GetStatString(bonusStat), bonusValue, durationInSeconds / 60);

        return res;
    }
}
