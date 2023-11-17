using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ExecutableItem, IStatusEffect
{
    [field:SerializeField] public Sprite Icon { get; protected set; }
    [field:SerializeField] public float Duration { get; protected set;}
    public float Timer { get; protected set; }

    
    public override bool Execute(Unit caster)
    {
        this.caster = caster;
        if (caster.TryGetComponent(out Potion potion))
        {
            Destroy(potion);
        }

        return true;
    }

    public override string[] ToString(Unit owner)
    {
        throw new System.NotImplementedException();
    }
}
