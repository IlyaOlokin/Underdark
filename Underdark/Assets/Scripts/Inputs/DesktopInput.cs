using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class DesktopInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action<State<EnemyState, StateEvent>> ShootInput;

    private Vector2 dir;
    
    void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetMouseButtonDown(0))
            ShootInput?.Invoke(null);
    }

    private void FixedUpdate()
    {
        PlayerMoveInput();
    }

    public void PlayerMoveInput()
    {
        MoveInput?.Invoke(dir);
    }
}
