using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "LevelConfig", fileName = "LevelConfig")]

public class  LevelConfigSO : ScriptableObject
{
    public List<FloorSetup> Floors;
}

[Serializable]
public class FloorSetup
{
    public int LevelsCount; 
    public int LevelSize; 
    public List<DamageTypeList> DamageTypesLists;
}

[Serializable]
public class DamageTypeList
{
    public List<DamageType> DamageTypes;
}
