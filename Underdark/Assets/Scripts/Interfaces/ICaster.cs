using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICaster
{
    public List<ActiveAblity> ActiveAbilities{ get;  }
    public List<float> ActiveAbilitiesCD { get;  }

    void ExecuteActiveAbility(int index);
}
