using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elixir : ExecutableItem
{
    public override void Execute(Unit caster)
    {
        if (caster.TryGetComponent(out Elixir elixir))
        {
            Destroy(elixir);
        }
    }

    public override string[] ToString(Unit owner)
    {
        throw new System.NotImplementedException();
    }
}
