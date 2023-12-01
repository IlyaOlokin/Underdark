using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarmScroll : Scroll
{
    [SerializeField] private HarmType harmType;
    
    public override bool Execute(Unit caster)
    {
        base.Execute(caster);

        var harms = caster.transform.GetComponents<HarmOverTime>();
        foreach (var harm in harms)
        {
            if (harm.HarmInfo.HarmType != harmType) continue;
            Destroy(harm);
            break;
        }
        
        return true;
    }

    public override string[] ToString(Unit owner)
    {
        string[] res = new string[1];

        res[0] = description;

        return res;
    }
}
