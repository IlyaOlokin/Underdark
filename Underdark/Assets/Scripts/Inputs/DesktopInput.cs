using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class DesktopInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action ActiveAbilityInput1;
    public event Action ActiveAbilityInput2;
    public event Action ActiveAbilityInput3;
    public event Action ActiveAbilityInput4;

    private Vector2 dir;
    
    void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetMouseButtonDown(0))
            ShootInput?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActiveAbilityInput1?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActiveAbilityInput2?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActiveAbilityInput3?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ActiveAbilityInput4?.Invoke();
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
