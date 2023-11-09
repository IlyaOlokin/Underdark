using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ExecutableItem
{
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
