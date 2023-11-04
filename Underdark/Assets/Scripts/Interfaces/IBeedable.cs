using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBleedable
{
    void GetBleed(BleedInfo bleedInfo, Unit caster, GameObject visual);
}
