using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebuffInfo : ScriptableObject
{
    [Range(0f, 1f)] public float chance;
    protected Unit caster;
    
    public virtual void Execute(Unit damageable)
    {
        
    }
    
    public virtual void Execute(IPoisonable poisonable)
    {
        
    }
    
    public virtual void Execute(IStunable stunable)
    {
        
    }
}
