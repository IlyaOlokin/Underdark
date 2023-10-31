using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitStats
{
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

    [field: Header("Stats")]
    [field: SerializeField] public int Level { get; private set; }

    public int FreePoints;
    [field: SerializeField] public int Strength { get; set; }
    [field: SerializeField] public int Dexterity { get; set; }
    [field: SerializeField] public int Intelligence { get; set; }

    public int StrDex => Strength + Dexterity;
    public int DexInt => Dexterity + Intelligence;
    public int IntStr => Intelligence + Strength;
    public int AllStats => Strength + Dexterity + Intelligence;

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
            TryLevelUp();
        }
    }

    public int ExpToLevelUp()
    {
        if (Level - 1 == expNeeded.Count) return -1;
        return expNeeded[Level - 1];
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
}
