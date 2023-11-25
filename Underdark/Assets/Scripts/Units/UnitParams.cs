using System;
using UnityEngine;

[Serializable]
public class UnitParams
{
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

    public float GetDamageAmplification(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physic => basePhysicDmgAmplification,
            DamageType.Chaos => baseChaosDmgAmplification,
            DamageType.Fire => baseFireDmgAmplification,
            DamageType.Air => baseAirDmgAmplification,
            DamageType.Water => baseWaterDmgAmplification,
            DamageType.Cold => baseColdDmgAmplification,
            DamageType.Electric => baseElectricDmgAmplification,
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }
    
    public float GetDamageResistance(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physic => basePhysicResistance,
            DamageType.Chaos => baseChaosResistance,
            DamageType.Fire => baseFireResistance,
            DamageType.Air => baseAirResistance,
            DamageType.Water => baseWaterResistance,
            DamageType.Cold => baseColdResistance,
            DamageType.Electric => baseElectricResistance,
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }
}
