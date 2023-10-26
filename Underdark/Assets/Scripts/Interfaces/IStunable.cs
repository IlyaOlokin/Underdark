using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStunable
{
    bool IsStunned { get; }
    
    bool GetStunned(StunInfo stunInfo);

    void GetUnStunned();

}
