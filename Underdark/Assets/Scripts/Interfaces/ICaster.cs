using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICaster
{
    int MaxMana { get; }
    public List<float> ActiveAbilitiesCD { get; }

    void ExecuteActiveAbility(int index);
}
