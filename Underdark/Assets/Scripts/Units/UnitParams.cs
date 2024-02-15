using System;
using UnityEngine;

[Serializable]
public class UnitParams
{
    private Unit unit;
    
    [Header("Base Damage Amplification")]
    [SerializeField] private float basePhysicDmgAmplification;
    [SerializeField] private float baseChaosDmgAmplification;
    [SerializeField] private float baseFireDmgAmplification;
    [SerializeField] private float baseAirDmgAmplification;
    [SerializeField] private float baseWaterDmgAmplification;
    [SerializeField] private float baseColdDmgAmplification;
    [SerializeField] private float baseElectricDmgAmplification;
    
    [Header("Base Damage Resistance")]
    [SerializeField] private float basePhysicResistance;
    [SerializeField] private float baseChaosResistance;
    [SerializeField] private float baseFireResistance;
    [SerializeField] private float baseAirResistance;
    [SerializeField] private float baseWaterResistance;
    [SerializeField] private float baseColdResistance;
    [SerializeField] private float baseElectricResistance;

    [Header("Evasion")] 
    [SerializeField] private float baseEvasionChance;

    [Header("Regen")]
    [SerializeField] [Range(0f, 1f)] private float baseHPRegenPercent;
    [SerializeField] [Range(0f, 1f)] private float baseMPRegenPercent;

    private float HPRegenAmplification;
    private float MPRegenAmplification;

    public float SlowAmount { get; private set; }
    public float AllDmgAmplification { get; private set; }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        ApplySlow(1f);

        unit.Inventory.OnEquipmentChanged += CashParams;
        CashParams();
    }

    public float GetDamageAmplification(DamageType damageType)
    {
        float dmgAmpl = 0;
        
        foreach (var passive in unit.GetAllGearPassives<DamageAmplificationSO>())
        {
            if (passive.DamageType == damageType)
            {
                dmgAmpl += passive.Value;
            }
        }
        
        return damageType switch
        {
            DamageType.Physic => (basePhysicDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Chaos => (baseChaosDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Fire => (baseFireDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Air => (baseAirDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Water => (baseWaterDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Cold => (baseColdDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Electric => (baseElectricDmgAmplification + dmgAmpl + 1) * (AllDmgAmplification + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }
    
    public float GetDamageResistance(DamageType damageType)
    {
        float dmgRes = 0;
        
        foreach (var passive in unit.GetAllGearPassives<DamageResistSO>())
        {
            if (passive.DamageType == damageType)
            {
                dmgRes += passive.Value;
            }
        }
        
        return damageType switch
        {
            DamageType.Physic => CalculateResist(basePhysicResistance, dmgRes),
            DamageType.Chaos => CalculateResist(baseChaosResistance, dmgRes),
            DamageType.Fire => CalculateResist(baseFireResistance, dmgRes),
            DamageType.Air => CalculateResist(baseAirResistance, dmgRes),
            DamageType.Water => CalculateResist(baseWaterResistance, dmgRes),
            DamageType.Cold => CalculateResist(baseColdResistance, dmgRes),
            DamageType.Electric => CalculateResist(baseElectricResistance, dmgRes),
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }

    private float CalculateResist(float baseRes, float bonusRes)
    {
        var a = Mathf.Clamp(1 - (baseRes + bonusRes), 0, 2);
        return Mathf.Clamp(1 - (baseRes + bonusRes), 0, 2);
    }

    public float GetEvasionChance()
    {
        var hitChance = 1 - baseEvasionChance;
        foreach (var evasionAmplification in unit.GetAllGearPassives<EvasionAmplificationSO>())
            hitChance *= 1 - evasionAmplification.EvasionChance;
        
        hitChance = Mathf.Clamp(hitChance, 0.05f, 1f);
        
        return 1 - hitChance;
    }

    public float GetRegenPerSecond(RegenType regenType)
    {
        return regenType switch
        {
            RegenType.HP => unit.MaxHP * baseHPRegenPercent * HPRegenAmplification,
            RegenType.MP => unit.MaxMana * baseMPRegenPercent * MPRegenAmplification,
            _ => throw new ArgumentOutOfRangeException(nameof(regenType), regenType, null)
        };
    }

    public void ApplySlow(float slow)
    {
        var newSlow = 1 - slow;
        if (newSlow < 0) newSlow = 0;
        
        SlowAmount = newSlow;
    }

    public void AddAllDamageAmplification(float dmgAmplification)
    {
        AllDmgAmplification += dmgAmplification;
    }

    private void CashParams()
    {
        SetRegeneration();
    }

    private void SetRegeneration()
    {
        var hpRegenAmpl = 0f;
        var mpRegenAmpl = 0f;
        
        foreach (var passive in unit.GetAllGearPassives<RegenAmplificationSO>())
        {
            switch (passive.RegenType)
            {
                case RegenType.HP:
                    hpRegenAmpl += passive.Value;
                    break;
                case RegenType.MP:
                    mpRegenAmpl += passive.Value;
                    break;
                case RegenType.Both:
                    hpRegenAmpl += passive.Value;
                    mpRegenAmpl += passive.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        HPRegenAmplification = 1 + hpRegenAmpl;
        MPRegenAmplification = 1 + mpRegenAmpl;
    }
}
