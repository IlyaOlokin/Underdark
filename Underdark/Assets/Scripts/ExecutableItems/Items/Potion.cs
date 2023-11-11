using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ExecutableItem, IBuff
{
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public float Duration { get; protected set;}
    public float Timer { get; protected set; }

    
    public override void Execute(Unit caster)
    {
        this.caster = caster;
        if (caster.TryGetComponent(out Potion potion))
        {
            Destroy(potion);
        }
    }

    public override string[] ToString(Unit owner)
    {
        throw new System.NotImplementedException();
    }
}
