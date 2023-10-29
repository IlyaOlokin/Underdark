using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebuffInfo : ScriptableObject
{
    [Range(0f, 1f)] public float chance;
    protected Unit caster;
    
    public virtual void Execute(IAttacker caster, Unit receiver)
    {
        throw new NotImplementedException();

    }
}
