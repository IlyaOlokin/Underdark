using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : ExecutableItem
{
    public override void Execute(Unit caster)
    {
        if (caster.TryGetComponent(out Scroll scroll))
        {
            Destroy(scroll);
        }
    }

    public override string[] ToString(Unit owner)
    {
        throw new System.NotImplementedException();
    }
}
