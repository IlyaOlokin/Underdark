using System;
using UnityEngine;

[Serializable]
public class Requirements
{
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int Strength { get; private set; }
    [field: SerializeField] public int Dexterity { get; private set; }
    [field: SerializeField] public int Intelligence { get; private set; }
    [field: SerializeField] public int StrDex { get; private set; }
    [field: SerializeField] public int DexInt { get; private set; }
    [field: SerializeField] public int IntStr { get; private set; }
    [field: SerializeField] public int AllStats { get; private set; }
}
