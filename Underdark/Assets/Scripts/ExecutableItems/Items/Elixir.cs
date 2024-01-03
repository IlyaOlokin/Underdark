using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elixir : ExecutableItem, IStatusEffect
{
    [field:SerializeField] public Sprite Icon { get; protected set; }
    [field:SerializeField] public float Duration { get; protected set;}
    public float Timer { get; protected set; }

    
    public override bool Execute(Unit caster)
    {
        if (caster.TryGetComponent(out Elixir elixir))
        {
            Destroy(elixir);
        }

        return true;
    }

    public override string[] ToString(Unit owner)
    {
        throw new System.NotImplementedException();
    }
    
    public override string[] ToStringAdditional()
    {
        List<string> res = new List<string>();
        
        res.Add("You can have only one active elixir effect at the time.");

        return res.ToArray();
    }
}
