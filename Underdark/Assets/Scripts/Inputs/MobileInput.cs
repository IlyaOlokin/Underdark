using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityHFSM;
using Zenject;

public class MobileInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action<int> ActiveAbilityInput;
    public event Action<int> ActiveAbilityHoldStart;
    public event Action<int> ActiveAbilityHoldEnd;
    public event Action<int> ExecutableItemInput;

    private PlayerInputUI inputUI;
    
    [Inject]
    private void Construct(PlayerInputUI inputUI)
    {
        this.inputUI = inputUI;
    }

    private void Awake()
    {
        inputUI.shootButton.GetComponent<HoldButton>().OnButtonHold += ButtonHold;
        

        inputUI.activeAbilityButtons[0].OnButtonHoldStart += ActiveAbilityHoldStart1;
        inputUI.activeAbilityButtons[0].OnButtonHoldEnd += ActiveAbilityHoldEnd1;
        inputUI.activeAbilityButtons[0].OnButtonClick += ActiveAbility1;
        
        inputUI.activeAbilityButtons[1].OnButtonHoldStart += ActiveAbilityHoldStart2;
        inputUI.activeAbilityButtons[1].OnButtonHoldEnd += ActiveAbilityHoldEnd2;
        inputUI.activeAbilityButtons[1].OnButtonClick += ActiveAbility2;
        
        inputUI.activeAbilityButtons[2].OnButtonHoldStart += ActiveAbilityHoldStart3;
        inputUI.activeAbilityButtons[2].OnButtonHoldEnd += ActiveAbilityHoldEnd3;
        inputUI.activeAbilityButtons[2].OnButtonClick += ActiveAbility3;
        
        inputUI.activeAbilityButtons[3].OnButtonHoldStart += ActiveAbilityHoldStart4;
        inputUI.activeAbilityButtons[3].OnButtonHoldEnd += ActiveAbilityHoldEnd4;
        inputUI.activeAbilityButtons[3].OnButtonClick += ActiveAbility4;

        
        inputUI.executableSlotsHandler.executableButtons[0].onClick.AddListener(ExecuteExecutableSlot1);
        inputUI.executableSlotsHandler.executableButtons[1].onClick.AddListener(ExecuteExecutableSlot2);
    }

    void FixedUpdate()
    {
        PlayerMoveInput();
    }

    private void ButtonHold()
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
    
    private void ActiveAbilityHoldStart1()
    {
        ActiveAbilityHoldStart?.Invoke(0);
    }
    
    private void ActiveAbilityHoldEnd1()
    {
        ActiveAbilityHoldEnd?.Invoke(0);
    }
    
    private void ActiveAbilityHoldStart2()
    {
        ActiveAbilityHoldStart?.Invoke(1);
    }
    
    private void ActiveAbilityHoldEnd2()
    {
        ActiveAbilityHoldEnd?.Invoke(1);
    }
    
    private void ActiveAbilityHoldStart3()
    {
        ActiveAbilityHoldStart?.Invoke(2);
    }
    
    private void ActiveAbilityHoldEnd3()
    {
        ActiveAbilityHoldEnd?.Invoke(2);
    }
    
    private void ActiveAbilityHoldStart4()
    {
        ActiveAbilityHoldStart?.Invoke(3);
    }
    
    private void ActiveAbilityHoldEnd4()
    {
        ActiveAbilityHoldEnd?.Invoke(3);
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
