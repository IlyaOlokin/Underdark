using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedScroll : Scroll
{
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        if (caster.transform.TryGetComponent(out Bleed poison))
            Destroy(poison);
    }

    public override string[] ToString(Unit owner)
    {
        string[] res = new string[1];

        res[0] = description;

        return res;
    }
}
