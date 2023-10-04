using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMover 
{
    int MoveSpeed { get; }

    void Move(Vector3 dir);
}
