using System;
using UnityEngine;
using UnityHFSM;

public interface IInput
{
    event Action<Vector3> MoveInput;
    event Action ShootInput;
    event Action<int> ActiveAbilityInput;
    event Action<int> ExecutableItemInput;

    void PlayerMoveInput();
}
