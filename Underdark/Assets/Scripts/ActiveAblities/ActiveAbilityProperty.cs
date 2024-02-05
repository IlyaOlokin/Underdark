using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveAbilityProperty<T>
{
    [SerializeField] private List<T> values;

    public T GetValue(int level)
    {
        level -= 1;
        
        return values.Count > level ? values[level] : values[^1];
    }
}
