using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecutableItem : MonoBehaviour
{
    [SerializeField] protected string description;
    
    public abstract void Execute(Unit caster);

    public new abstract string[] ToString();
}
