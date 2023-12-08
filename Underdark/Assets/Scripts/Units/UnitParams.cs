using System;
using UnityEngine;

[Serializable]
public class UnitParams
{
    private Unit unit;
    
    [Header("Base Damage Amplification")]
    [SerializeField] public float basePhysicDmgAmplification;
    [SerializeField] public float baseChaosDmgAmplification;
    [SerializeField] public float baseFireDmgAmplification;
    [SerializeField] public float baseAirDmgAmplification;
    [SerializeField] public float baseWaterDmgAmplification;
    [SerializeField] public float baseColdDmgAmplification;
    [SerializeField] public float baseElectricDmgAmplification;
    
    [Header("Base Damage Resistance")]
    [SerializeField] public float basePhysicResistance;
    [SerializeField] public float baseChaosResistance;
    [SerializeField] public float baseFireResistance;
    [SerializeField] public float baseAirResistance;
    [SerializeField] public float baseWaterResistance;
    [SerializeField] public float baseColdResistance;
    [SerializeField] public float baseElectricResistance;

    [Header("Evasion")] 
    [SerializeField] private float baseEvasionChance;

    public float SlowAmount { get; private set; }
    public float AllDmgAmplification { get; private set; }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        ApplySlow(1f);
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
            DamageType.Physic => (basePhysicResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Chaos => (baseChaosResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Fire => (baseFireResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Air => (baseAirResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Water => (baseWaterResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Cold => (baseColdResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
            DamageType.Electric => (baseElectricResistance + dmgAmpl + 1) * (AllDmgAmplification + 1),
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
            DamageType.Physic => CalculateResist(basePhysicDmgAmplification, dmgRes),
            DamageType.Chaos => CalculateResist(baseChaosDmgAmplification, dmgRes),
            DamageType.Fire => CalculateResist(baseFireDmgAmplification, dmgRes),
            DamageType.Air => CalculateResist(baseAirDmgAmplification, dmgRes),
            DamageType.Water => CalculateResist(baseWaterDmgAmplification, dmgRes),
            DamageType.Cold => CalculateResist(baseColdDmgAmplification, dmgRes),
            DamageType.Electric => CalculateResist(baseElectricDmgAmplification, dmgRes),
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }

    private float CalculateResist(float baseRes, float bonusRes)
    {
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

    public void ApplySlow(float slow)
    {
        SlowAmount = slow;
    }

    public void AddAllDamageAmplification(float dmgAmplofocation)
    {
        AllDmgAmplification += dmgAmplofocation;
    }
}
