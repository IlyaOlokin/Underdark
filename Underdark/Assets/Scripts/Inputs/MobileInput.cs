using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;

public class MobileInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    public event Action ActiveAbilityInput1;
    public event Action ActiveAbilityInput2;
    public event Action ActiveAbilityInput3;
    public event Action ActiveAbilityInput4;
    
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Button shootButton;
    [SerializeField] private List<Button> activeAbilityButtons;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        shootButton.onClick.AddListener(Shoot);
        activeAbilityButtons[0].onClick.AddListener(ActiveAbility1);
        activeAbilityButtons[1].onClick.AddListener(ActiveAbility2);
        activeAbilityButtons[2].onClick.AddListener(ActiveAbility3);
        activeAbilityButtons[3].onClick.AddListener(ActiveAbility4);
        canvas.worldCamera = Camera.main;
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
        ActiveAbilityInput1?.Invoke();
    }
    private void ActiveAbility2()
    {
        ActiveAbilityInput2?.Invoke();
    }
    private void ActiveAbility3()
    {
        ActiveAbilityInput3?.Invoke();
    }
    private void ActiveAbility4()
    {
        ActiveAbilityInput4?.Invoke();
    }
    
    
    public void PlayerMoveInput()
    {
        Vector2 inputDir = new Vector2(
            joystick.Horizontal,
            joystick.Vertical
        );
        
        MoveInput?.Invoke(inputDir);
    }
}
