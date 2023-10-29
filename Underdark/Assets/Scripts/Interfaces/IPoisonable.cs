using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoisonable
{
    void GetPoisoned(PoisonInfo poisonInfo, Unit caster);
}
