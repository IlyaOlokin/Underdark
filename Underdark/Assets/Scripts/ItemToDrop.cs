using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemToDrop
{
    public Item Item;
    public int ItemAmount;
    public float ChanceToDropLeft;
    public float ChanceToDropRight;
}
