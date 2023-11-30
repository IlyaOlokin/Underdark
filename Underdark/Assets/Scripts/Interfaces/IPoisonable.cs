using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoisonable
{
    void GetPoisoned(HarmInfo harmInfo, Unit caster, GameObject visual);
}
