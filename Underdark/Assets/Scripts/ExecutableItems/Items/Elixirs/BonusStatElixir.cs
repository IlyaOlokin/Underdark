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
    
    public override bool Execute(Unit caster)
    {
        base.Execute(caster);
        var comp = caster.AddComponent<BonusStatElixir>();
        comp.Init(bonusStat, bonusValue, Duration, Icon, caster);
        return true;
    }

    public void Init(BaseStat bonusStat, int bonusValue, float duration, Sprite icon, Unit caster)
    {
        this.bonusStat = bonusStat;
        this.bonusValue = bonusValue;
        Duration = duration;
        HandleCD(duration);
        Icon = icon;
        this.caster = caster;
        caster.Stats.ApplyBonusStat(bonusStat, bonusValue);
        caster.ReceiveStatusEffect(this);
    }

    private void HandleCD(float duration)
    {
        if (ElixirStaticData.ElixirCD > 0)
            Timer = ElixirStaticData.ElixirCD;
        else
            Timer = duration;
        ElixirStaticData.ElixirCD = -1f;
    }

    private void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer <= 0)
            Destroy(this);
    }

    private void OnDestroy()
    {
        caster.LooseStatusEffect(this);
        caster.Stats.ApplyBonusStat(bonusStat, -bonusValue);
    }
    
    public override string[] ToString(Unit owner)
    {
        var res = new string[1];
        
        res[0] = string.Format(description, UnitStats.GetStatString(bonusStat), bonusValue, Duration / 60);

        return res;
    }
}
