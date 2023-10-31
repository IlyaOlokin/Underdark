using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;
using Zenject;

public class MobileInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action<int> ActiveAbilityInput;
    public event Action<int> ExecutableItemInput;

    private PlayerInputUI inputUI;
    
    [Inject]
    private void Construct(PlayerInputUI inputUI)
    {
        this.inputUI = inputUI;
    }

    private void Awake()
    {
        inputUI.shootButton.onClick.AddListener(Shoot);
        inputUI.activeAbilityButtons[0].onClick.AddListener(ActiveAbility1);
        inputUI.activeAbilityButtons[1].onClick.AddListener(ActiveAbility2);
        inputUI.activeAbilityButtons[2].onClick.AddListener(ActiveAbility3);
        inputUI.activeAbilityButtons[3].onClick.AddListener(ActiveAbility4);
        
        inputUI.executableSlotsHandler.executableSlots[0].onClick.AddListener(ExecuteExecutableSlot1);
        inputUI.executableSlotsHandler.executableSlots[1].onClick.AddListener(ExecuteExecutableSlot2);
    }

    void FixedUpdate()
    {
        PlayerMoveInput();
    }

    private void Shoot()
    {
        ShootInput?.Invoke();
    }
    
    private void ActiveAbility1()
    {
        ActiveAbilityInput?.Invoke(0);
    }
    private void ActiveAbility2()
    {
        ActiveAbilityInput?.Invoke(1);
    }
    private void ActiveAbility3()
    {
        ActiveAbilityInput?.Invoke(2);
    }
    private void ActiveAbility4()
    {
        ActiveAbilityInput?.Invoke(3);
    }

    private void ExecuteExecutableSlot1()
    {
        ExecutableItemInput?.Invoke(0);
    }
    
    private void ExecuteExecutableSlot2()
    {
        ExecutableItemInput?.Invoke(1);
    }
    
    
    
    public void PlayerMoveInput()
    {
        Vector2 inputDir = new Vector2(
            inputUI.joystick.Horizontal,
            inputUI.joystick.Vertical
        );
        
        MoveInput?.Invoke(inputDir);
    }
}
