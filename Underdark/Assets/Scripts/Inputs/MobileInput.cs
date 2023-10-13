using System;
using UnityEngine;
using UnityEngine.UI;
using UnityHFSM;

public class MobileInput : MonoBehaviour, IInput
{
    public event Action<Vector3> MoveInput;
    public event Action<State<EnemyState, StateEvent>> ShootInput;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Button shootButton;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        shootButton.onClick.AddListener(Shoot);
        canvas.worldCamera = Camera.main;
    }

    void FixedUpdate()
    {
        PlayerMoveInput();
    }

    private void Shoot()
    {
        ShootInput?.Invoke(null);
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
