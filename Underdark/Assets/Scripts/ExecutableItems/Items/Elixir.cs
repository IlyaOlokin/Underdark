using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elixir : ExecutableItem, IBuff
{
    [field:SerializeField] public Sprite Icon { get; protected set; }
    [field:SerializeField] public float Duration { get; protected set;}
    public float Timer { get; protected set; }

    
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
