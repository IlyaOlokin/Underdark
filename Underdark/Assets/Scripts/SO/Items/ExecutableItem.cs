using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecutableItem : MonoBehaviour
{
    [Multiline][SerializeField] protected string description;
    protected Unit caster;
    
    public abstract bool Execute(Unit caster);

    public abstract string[] ToString(Unit owner);

    public virtual string[] ToStringAdditional() => new string[]{};
}
