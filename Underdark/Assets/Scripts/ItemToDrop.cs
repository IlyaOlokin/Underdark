using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemToDrop<T>
{
    public T Item;
    public int ItemAmount;
    public float ChanceToDropLeft;
    public float ChanceToDropRight;
}
