using System;
using UnityEngine;

[Serializable]
public class UnitStats
{
    [field: SerializeField] public int Level { get; private set; }

    public int FreePoints;
    [field: SerializeField] public int Strength { get; set; }
    [field: SerializeField] public int Dexterity { get; set; }
    [field: SerializeField] public int Intelligence { get; set; }

    public int StrDex => Strength + Dexterity;
    public int DexInt => Dexterity + Intelligence;
    public int IntStr => Intelligence + Strength;
    public int AllStats => Strength + Dexterity + Intelligence;

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
}
