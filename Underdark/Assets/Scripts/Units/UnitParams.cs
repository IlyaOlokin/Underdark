using System;
using UnityEngine;

[Serializable]
public class UnitParams
{
    private Unit unit;
    
    [Header("Base Damage Amplification")]
    [SerializeField] public float basePhysicDmgAmplification = 1;
    [SerializeField] public float baseChaosDmgAmplification = 1;
    [SerializeField] public float baseFireDmgAmplification = 1;
    [SerializeField] public float baseAirDmgAmplification = 1;
    [SerializeField] public float baseWaterDmgAmplification = 1;
    [SerializeField] public float baseColdDmgAmplification = 1;
    [SerializeField] public float baseElectricDmgAmplification = 1;
    
    [Header("Base Damage Resistance")]
    [SerializeField] public float basePhysicResistance = 1;
    [SerializeField] public float baseChaosResistance = 1;
    [SerializeField] public float baseFireResistance = 1;
    [SerializeField] public float baseAirResistance = 1;
    [SerializeField] public float baseWaterResistance = 1;
    [SerializeField] public float baseColdResistance = 1;
    [SerializeField] public float baseElectricResistance = 1;

    [Header("Evasion")] 
    [SerializeField] private float baseEvasionChance;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
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
            DamageType.Physic => basePhysicDmgAmplification * (1 + dmgAmpl),
            DamageType.Chaos => baseChaosDmgAmplification * (1 + dmgAmpl),
            DamageType.Fire => baseFireDmgAmplification * (1 + dmgAmpl),
            DamageType.Air => baseAirDmgAmplification * (1 + dmgAmpl),
            DamageType.Water => baseWaterDmgAmplification * (1 + dmgAmpl),
            DamageType.Cold => baseColdDmgAmplification * (1 + dmgAmpl),
            DamageType.Electric => baseElectricDmgAmplification * (1 + dmgAmpl),
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
            DamageType.Physic => basePhysicResistance * (1 + dmgRes),
            DamageType.Chaos => baseChaosResistance * (1 + dmgRes),
            DamageType.Fire => baseFireResistance * (1 + dmgRes),
            DamageType.Air => baseAirResistance * (1 + dmgRes),
            DamageType.Water => baseWaterResistance * (1 + dmgRes),
            DamageType.Cold => baseColdResistance * (1 + dmgRes),
            DamageType.Electric => baseElectricResistance * (1 + dmgRes),
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }

    public float GetEvasionChance()
    {
        var hitChance = 1 - baseEvasionChance;
        foreach (var evasionAmplification in unit.GetAllGearPassives<EvasionAmplificationSO>())
            hitChance *= 1 - evasionAmplification.EvasionChance;
        
        hitChance = Mathf.Clamp(hitChance, 0.05f, 1f);
        
        return 1 - hitChance;
    }
}
