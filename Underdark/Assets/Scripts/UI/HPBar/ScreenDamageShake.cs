using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDamageShake : MonoBehaviour
{
    [SerializeField] private float shakeTrashHold = 0.25f;
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerUIHealthBar healthBar;
    private float oldValue;

    private void OnEnable()
    {
        healthBar.OnValueChanged += TryScreenShake;
    }

    private void TryScreenShake(float newValue)
    {
        if (oldValue - newValue > slider.maxValue * shakeTrashHold)
            CameraShake.Instance.Shake(transform.position);

        oldValue = newValue;
    }
    
    private void OnDisable()
    {
        healthBar.OnValueChanged -= TryScreenShake;
    }
}
