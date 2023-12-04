using System;
using UnityEngine;
using UnityHFSM;

public interface IInput
{
    bool IsEnabled { get; set; }
    
    event Action<Vector3> MoveInput;
    event Action ShootInput;
    event Action<int> ActiveAbilityInput;
    public event Action<int> ActiveAbilityHoldStart;
    public event Action<int> ActiveAbilityHoldEnd;
    event Action<int> ExecutableItemInput;

    void PlayerMoveInput();
}
