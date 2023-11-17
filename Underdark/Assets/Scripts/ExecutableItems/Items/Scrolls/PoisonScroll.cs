using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonScroll : Scroll
{
    public override bool Execute(Unit caster)
    {
        base.Execute(caster);
        if (caster.transform.TryGetComponent(out Poison poison))
            Destroy(poison);
        
        return true;
    }

    public override string[] ToString(Unit owner)
    {
        string[] res = new string[1];

        res[0] = description;

        return res;
    }
}
