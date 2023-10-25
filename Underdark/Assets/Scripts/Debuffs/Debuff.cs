using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff
{
    public bool IsExpired => duration <= 0;

    protected float duration;

    public virtual void Update()
    {
        
    }
}
