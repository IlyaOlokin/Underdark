using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICaster
{
    int MaxMana { get; }
    //public List<ActiveAbility> ActiveAbilities{ get; }
    public List<float> ActiveAbilitiesCD { get; }

    void ExecuteActiveAbility(int index);
}
