using System;
using UnityEngine;
using UnityHFSM;

public interface IInput
{
    event Action<Vector3> MoveInput;
    event Action<State<EnemyState, StateEvent>> ShootInput;
    void PlayerMoveInput();
}
