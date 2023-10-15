using System;
using UnityEngine;
using UnityHFSM;

public interface IInput
{
    event Action<Vector3> MoveInput;
    event Action ShootInput;
    event Action ActiveAbilityInput1;
    event Action ActiveAbilityInput2;
    event Action ActiveAbilityInput3;
    event Action ActiveAbilityInput4;
    void PlayerMoveInput();
}
