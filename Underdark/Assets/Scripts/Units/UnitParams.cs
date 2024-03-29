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
    [SerializeField] private float baseColdDmgAmplification;
    [SerializeField] private float baseElectricDmgAmplification;
    
    [Header("Base Damage Resistance")]
    [SerializeField] private float basePhysicResistance;
    [SerializeField] private float baseChaosResistance;
    [SerializeField] private float baseFireResistance;
    [SerializeField] private float baseColdResistance;
    [SerializeField] private float baseElectricResistance;

    [Header("Evasion")] 
    [SerializeField] private float baseEvasionChance;

    [Header("Regen")]
    [SerializeField] [Range(0f, 1f)] private float baseHPRegenPercent;
    [SerializeField] [Range(0f, 1f)] private float baseMPRegenPercent;
    
    private float HPRegenPercent;
    private float MPRegenPercent;

    private float HPRegenAmplification;
    private float MPRegenAmplification;

    public float SlowDebuffAmount { get; private set; } = 1f;
    public float MoveSpeedMultiplier { get; private set; } = 1f;
    public float CdAbilitySpeedMultiplier { get; private set; } = 1f;
    public float CdDexteritySpeedMultiplier { get; private set; } = 1f;

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        ApplySlowDebuff(0);

        unit.Inventory.OnEquipmentChanged += CashParams;
        unit.OnUnitPassivesChanged += CashParams;
        unit.Stats.OnStatsChanged += CashParams;
        CashParams();
    }

    public float GetDamageAmplification(DamageType damageType)
    {
        float dmgAmpl = 0;
        
        foreach (var passive in unit.GetAllPassives<DamageAmplificationSO>())
        {
            if (passive.DamageType == damageType)
            {
                dmgAmpl += passive.Value;
            }
        }

        float allDmgAmpl = 0;
        foreach (var passive in unit.GetAllPassives<AllDamageAmplificationSO>())
        {
            allDmgAmpl += passive.Value;
        }
        
        return damageType switch
        {
            DamageType.Physic => (basePhysicDmgAmplification + dmgAmpl + 1) * (allDmgAmpl + 1),
            DamageType.Chaos => (baseChaosDmgAmplification + dmgAmpl + 1) * (allDmgAmpl + 1),
            DamageType.Fire => (baseFireDmgAmplification + dmgAmpl + 1) * (allDmgAmpl + 1),
            DamageType.Cold => (baseColdDmgAmplification + dmgAmpl + 1) * (allDmgAmpl + 1),
            DamageType.Electric => (baseElectricDmgAmplification + dmgAmpl + 1) * (allDmgAmpl + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null)
        };
    }
    
    public float GetDamageResistance(DamageType damageType)
    {
        float dmgRes = 0;
        
        foreach (var passive in unit.GetAllPassives<DamageResistSO>())
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
        foreach (var evasionAmplification in unit.GetAllPassives<EvasionAmplificationSO>())
            hitChance *= 1 - evasionAmplification.EvasionChance;
        
        hitChance = Mathf.Clamp(hitChance, 0.05f, 1f);
        
        return 1 - hitChance;
    }

    public float GetRegenPerSecond(RegenType regenType)
    {
        return regenType switch
        {
            RegenType.HP => unit.MaxHP * HPRegenPercent * HPRegenAmplification,
            RegenType.MP => unit.MaxMana * MPRegenPercent * MPRegenAmplification,
            _ => throw new ArgumentOutOfRangeException(nameof(regenType), regenType, null)
        };
    }

    public void ApplySlowDebuff(float slow)
    {
        var newSlow = 1 - slow;
        if (newSlow < 0) newSlow = 0;
        
        SlowDebuffAmount = newSlow;
    }

    private void CashParams()
    {
        SetRegenerationAmplification();
        SetBaseRegeneration();
        SetSpeedMultipliers();
    }

    private void SetSpeedMultipliers()
    {
        var moveSpeedMultiplier = 0f;
        var cdSpeedMultiplier = 0f;

        foreach (var passive in unit.GetAllPassives<MoveSpeedAmplificationSO>())
        {
            moveSpeedMultiplier += passive.Value;
        }
        foreach (var passive in unit.GetAllPassives<CdSpeedAmplificationSO>())
        {
            cdSpeedMultiplier += passive.Value;
        }
        
        MoveSpeedMultiplier = 1 + moveSpeedMultiplier;
        CdAbilitySpeedMultiplier = 1 + cdSpeedMultiplier;
        CdDexteritySpeedMultiplier = 1 + unit.Stats.GetTotalStatValue(BaseStat.Dexterity) / 100f;
    }

    private void SetRegenerationAmplification()
    {
        var hpRegenAmpl = 0f;
        var mpRegenAmpl = 0f;
        
        foreach (var passive in unit.GetAllPassives<RegenAmplificationSO>())
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
    
    private void SetBaseRegeneration()
    {
        var hpRegen = baseHPRegenPercent;
        var mpRegen = baseMPRegenPercent;
        
        foreach (var passive in unit.GetAllPassives<BaseRegenerationSO>())
        {
            switch (passive.RegenType)
            {
                case RegenType.HP:
                    if (passive.Value > hpRegen)
                        hpRegen = passive.Value;
                    break;
                case RegenType.MP:
                    if (passive.Value > mpRegen)
                        mpRegen = passive.Value;
                    break;
                case RegenType.Both:
                    if (passive.Value > hpRegen)
                        hpRegen = passive.Value;
                    if (passive.Value > mpRegen)
                        mpRegen = passive.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        HPRegenPercent = hpRegen;
        MPRegenPercent = mpRegen;
    }
}
