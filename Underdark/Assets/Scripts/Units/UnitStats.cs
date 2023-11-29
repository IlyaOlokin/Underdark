using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitStats
{
    [field: Header("Stats")]
    [field: SerializeField] public int Level { get; private set; }

    public int FreePoints;
    
    [field: SerializeField] public int Strength { get; set; }
    [field: SerializeField] public int Dexterity { get; set; }
    [field: SerializeField] public int Intelligence { get; set; }
    
    public int BonusStrength { get; set; }
    public int BonusDexterity { get; set; }
    public int BonusIntelligence { get; set; }

    public int StrDex => GetTotalStatValue(BaseStat.Strength) + GetTotalStatValue(BaseStat.Dexterity);
    public int DexInt => GetTotalStatValue(BaseStat.Dexterity) + GetTotalStatValue(BaseStat.Intelligence);
    public int IntStr => GetTotalStatValue(BaseStat.Intelligence) + GetTotalStatValue(BaseStat.Strength);
    public int AllStats => GetTotalStatValue(BaseStat.Strength) + GetTotalStatValue(BaseStat.Dexterity) + GetTotalStatValue(BaseStat.Intelligence);

    public event Action<bool> OnStatsChanged;
    public event Action OnLevelUp;

    [Header("Exp")] 
    [SerializeField] private int pointsPerLevel;
    [SerializeField] private List<int> expNeeded;
    public int CurrentExp { get; private set; }

    public void Reset()
    {
        Level = 1;
        FreePoints = 9;
        Strength = 1;
        Dexterity = 1;
        Intelligence = 1;
        CurrentExp = 0;
        OnStatsChanged?.Invoke(false);
    }

    public bool RequirementsMet(Requirements requirements)
    {
        return Level >= requirements.Level
               && ((Strength >= requirements.Strength
                    && Dexterity >= requirements.Dexterity
                    && Intelligence >= requirements.Intelligence)
                   && (StrDex >= requirements.StrDex
                   && DexInt >= requirements.DexInt
                   && IntStr >= requirements.IntStr
                   && AllStats >= requirements.AllStats));
    }

    public void CopyStatsFrom(UnitStats unitStats)
    {
        FreePoints = unitStats.FreePoints;
        Strength = unitStats.Strength;
        Dexterity = unitStats.Dexterity;
        Intelligence = unitStats.Intelligence;
        OnStatsChanged?.Invoke(false);
    }

    public void GetExp(int exp)
    {
        CurrentExp += exp;
        TryLevelUp();
    }

    private void TryLevelUp()
    {
        if (Level - 1 == expNeeded.Count) return;
        if (ExpToLevelUp() <= CurrentExp)
        {
            CurrentExp -= ExpToLevelUp();
            Level += 1;
            FreePoints += pointsPerLevel;
            OnLevelUp?.Invoke();
            TryLevelUp();
        }
    }

    public int ExpToLevelUp()
    {
        if (Level - 1 == expNeeded.Count) return -1;
        return expNeeded[Level - 1];
    }

    public void SetLevel(int level, int exp, int freePoints)
    {
        Level = level;
        GetExp(exp);
        FreePoints = freePoints;
    }

    public int GetTotalStatValue(BaseStat baseStat)
    {
        return baseStat switch
        {
            BaseStat.Strength => Strength + BonusStrength,
            BaseStat.Dexterity => Dexterity + BonusDexterity,
            BaseStat.Intelligence => Intelligence + BonusIntelligence,
            BaseStat.StrDex => StrDex + BonusStrength + BonusDexterity,
            BaseStat.DexInt => DexInt + BonusDexterity + BonusIntelligence,
            BaseStat.IntStr => IntStr + BonusIntelligence + BonusStrength,
            _ => throw new ArgumentOutOfRangeException(nameof(baseStat), baseStat, null)
        };
    }

    public int GetActualStat(BaseStat baseStat)
    {
        return baseStat switch
        {
            BaseStat.Strength => Strength,
            BaseStat.Dexterity => Dexterity,
            BaseStat.Intelligence => Intelligence,
            BaseStat.StrDex => StrDex,
            BaseStat.DexInt => DexInt,
            BaseStat.IntStr => IntStr,
            _ => throw new ArgumentOutOfRangeException(nameof(baseStat), baseStat, null)
        };
    }
    
    public void ApplyBonusStat(BaseStat baseStat, int value)
    {
        switch (baseStat)
        {
            case BaseStat.Strength:
                BonusStrength += value;
                break;
            case BaseStat.Dexterity:
                BonusDexterity += value;
                break;
            case BaseStat.Intelligence:
                BonusIntelligence += value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(baseStat), baseStat, null);
        }
        OnStatsChanged?.Invoke(false);
    }
    
    public static string GetStatString(BaseStat baseStat)
    {
        return baseStat switch
        {
            BaseStat.Strength => "Str",
            BaseStat.Dexterity => "Dex",
            BaseStat.Intelligence => "Int",
            BaseStat.StrDex => "(Str + Dex)",
            BaseStat.DexInt => "(Dex + Int)",
            BaseStat.IntStr => "(Int + Str)",
            _ => throw new ArgumentOutOfRangeException(nameof(baseStat), baseStat, null)
        };
    }

    public static bool operator ==(UnitStats a, UnitStats b)
    {
        return a.Strength == b.Strength 
               && a.Dexterity == b.Dexterity 
               && a.Intelligence == b.Intelligence;
    }

    public static bool operator !=(UnitStats a, UnitStats b)
    {
        return !(a == b);
    }
    
    protected bool Equals(UnitStats other)
    {
        return Strength == other.Strength && Dexterity == other.Dexterity && Intelligence == other.Intelligence;
    }
    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UnitStats)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Strength, Dexterity, Intelligence);
    }
}
