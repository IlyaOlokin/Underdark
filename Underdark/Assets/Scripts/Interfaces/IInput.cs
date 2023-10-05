using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInput
{
    event Action<Vector3> MoveInput;
    event Action ShootInput;
    void PlayerMoveInput();
}
