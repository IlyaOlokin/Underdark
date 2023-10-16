using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class DesktopInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action<int> ActiveAbilityInput;

    private Vector2 dir;
    
    void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetMouseButtonDown(0))
            ShootInput?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActiveAbilityInput?.Invoke(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActiveAbilityInput?.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActiveAbilityInput?.Invoke(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ActiveAbilityInput?.Invoke(3);
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
