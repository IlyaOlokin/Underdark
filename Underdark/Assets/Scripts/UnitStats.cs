using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitStats
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int Strength { get; private set; }
    [field: SerializeField] public int Dexterity { get; private set; }
    [field: SerializeField] public int Intelligence { get; private set; }

    public bool RequirementsMet(UnitStats requirements)
    {
        return Level >= requirements.Level &&
               Strength >= requirements.Strength &&
               Dexterity >= requirements.Dexterity &&
               Intelligence >= requirements.Intelligence;
    }
}
