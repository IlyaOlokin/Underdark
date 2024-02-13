using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScalableProperty<T> where T : new()
{
    [SerializeField] private List<T> values;

    public T GetValue(int level)
    {
        if (level > 0) level -= 1;

        if (values.Count == 0) return new T();
        
        return values.Count > level ? values[level] : values[^1];
    }
}
