using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelConfig", fileName = "LevelConfig")]

public class LevelConfigSO : ScriptableObject
{
    public List<FloorSetup> Floors;
}

[Serializable]
public class FloorSetup
{
    public int LevelsCount;
    public List<DamageType> DamageTypes;
}
