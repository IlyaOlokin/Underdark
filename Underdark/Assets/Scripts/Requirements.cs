using System;
using System.Text;
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

    public override string ToString()
    {
        var res = new StringBuilder();
        res.Append($"Level: {Level}");
        if (Strength != 0) res.Append($", Str: {Strength}");
        if (Dexterity != 0) res.Append($", Dex: {Dexterity}");
        if (Intelligence != 0) res.Append($", Int: {Intelligence}");
        if (StrDex != 0) res.Append($", Str + Dex: {StrDex}");
        if (DexInt != 0) res.Append($", Dex + Int: {DexInt}");
        if (IntStr != 0) res.Append($", Int + Str: {IntStr}");
        if (AllStats != 0) res.Append($", All stats: {AllStats}");
        
        return res.ToString();
    }
}
