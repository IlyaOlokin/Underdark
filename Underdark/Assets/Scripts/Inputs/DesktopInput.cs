using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;
using Zenject;

public class DesktopInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action<int> ActiveAbilityInput;
    public event Action<int> ActiveAbilityHoldStart;
    public event Action<int> ActiveAbilityHoldEnd;
    public event Action<int> ExecutableItemInput;

    private Vector2 dir;
    private PlayerInputUI inputUI;

    [Inject]
    private void Construct(PlayerInputUI inputUI)
    {
        this.inputUI = inputUI;
        foreach (var button in inputUI.activeAbilityButtons)
        {
            button.interactable = false;
        }
    }
    
    void Update()
    {
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (Input.GetMouseButton(0))
            ShootInput?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActiveAbilityInput?.Invoke(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActiveAbilityInput?.Invoke(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActiveAbilityInput?.Invoke(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ActiveAbilityInput?.Invoke(3);

        if (Input.GetKeyDown(KeyCode.Q))
            ExecutableItemInput?.Invoke(0);
        if (Input.GetKeyDown(KeyCode.E))
            ExecutableItemInput?.Invoke(1);
        

        if (Input.GetKeyDown(KeyCode.Tab))
            inputUI.inventoryButton.onClick?.Invoke();
        
        if (Input.GetKeyDown(KeyCode.I))
            inputUI.characterButton.onClick?.Invoke();

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
