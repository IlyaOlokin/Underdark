using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : ExecutableItem, IStatusEffect
{
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public float Duration { get; private set;}
    public float Timer { get; protected set; }

    public override bool Execute(Unit caster)
    {
        if (caster.TryGetComponent(out Scroll scroll))
        {
            Destroy(scroll);
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
        
        res.Add("You can have only one active scroll effect at the time.");

        return res.ToArray();
    }
}
