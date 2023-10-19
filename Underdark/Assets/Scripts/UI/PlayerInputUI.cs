using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityEngine.UI;


public class PlayerInputUI : MonoBehaviour
{
    public FloatingJoystick joystick;
    public Button shootButton;
    public List<Button> activeAbilityButtons;
    [NonSerialized] public Player player;
    
    private void Awake()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}
