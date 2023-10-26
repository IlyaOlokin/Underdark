using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemToDrop
{
    public Item Item;
    public int ItemAmount;
    [Range(0f, 1f)] public float ChanceToDrop;
}
