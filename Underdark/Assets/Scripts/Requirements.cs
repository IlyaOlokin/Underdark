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

    public string ToString(Unit owner)
    {
        var res = new StringBuilder();
        res.Append($"Level: {GetColor(owner.Stats.Level, Level)}{Level}</color>");
        if (Strength != 0) res.Append($", Str: {GetColor(owner.Stats.Strength, Strength)}{Strength}</color>");
        if (Dexterity != 0) res.Append($", Dex: {GetColor(owner.Stats.Dexterity, Dexterity)}{Dexterity}</color>");
        if (Intelligence != 0) res.Append($", Int: {GetColor(owner.Stats.Intelligence, Intelligence)}{Intelligence}</color>");
        if (StrDex != 0) res.Append($", Str + Dex: {GetColor(owner.Stats.StrDex, StrDex)}{StrDex}</color>");
        if (DexInt != 0) res.Append($", Dex + Int: {GetColor(owner.Stats.DexInt, DexInt)}{DexInt}</color>");
        if (IntStr != 0) res.Append($", Int + Str: {GetColor(owner.Stats.IntStr, IntStr)}{IntStr}</color>");
        if (AllStats != 0) res.Append($", All stats: {GetColor(owner.Stats.AllStats, AllStats)}{AllStats}</color>");
        
        return res.ToString();
    }

    private string GetColor(int ownerValue, int reqValue)
    {
        if (ownerValue < reqValue) return "<color=\"red\">";
        return "";
    }
}
