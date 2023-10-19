using System;
using UnityEngine;
using UnityHFSM;

public interface IInput
{
    event Action<Vector3> MoveInput;
    event Action ShootInput;
    event Func<int, float> ActiveAbilityInput;
    void PlayerMoveInput();
}
