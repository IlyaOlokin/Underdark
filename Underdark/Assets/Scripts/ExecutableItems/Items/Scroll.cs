using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : ExecutableItem, IBuff
{
    [field:SerializeField] public Sprite Icon { get; private set; }
    [field:SerializeField] public float Duration { get; private set;}
    public float Timer { get; protected set; }

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
