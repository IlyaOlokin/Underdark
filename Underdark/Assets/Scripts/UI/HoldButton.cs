using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnButtonHoldStart;
    public event Action OnButtonHold;
    public event Action OnButtonHoldEnd;
    private bool isPressed;

    private void Update()
    {
        if (isPressed) OnButtonHold?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        OnButtonHoldStart?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        OnButtonHoldEnd?.Invoke();
    }
}
