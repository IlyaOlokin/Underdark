using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecutableItem : MonoBehaviour
{
    [SerializeField] protected string description;
    protected Unit caster;
    
    public abstract void Execute(Unit caster);

    public abstract string[] ToString(Unit owner);
}
