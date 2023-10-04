using System;
using UnityEngine;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action ShootInput;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Button shootButton;

    private void Awake()
    {
        shootButton.onClick.AddListener(Shoot);
    }

    void FixedUpdate()
    {
        PlayerMoveInput();
    }

    public void Shoot()
    {
        ShootInput?.Invoke();
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
