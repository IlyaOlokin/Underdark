using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnButtonClick;
    public event Action OnButtonHoldStart;
    public event Action OnButtonHold;
    public event Action OnButtonHoldEnd;
    private bool isPressed;
    private bool holdStarted;

    [SerializeField] private float holdDelay = 0.5f;
    private float timer;

    public Button Button { get; private set; }

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void Update()
    {
        if (!isPressed) return;
        
        timer += Time.unscaledDeltaTime;
        
        if (timer < holdDelay) return;
        
        if (!holdStarted)
        {
            holdStarted = true;
            OnButtonHoldStart?.Invoke();
        }

        OnButtonHold?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        holdStarted = false;
        timer = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        if (timer >= holdDelay) OnButtonHoldEnd?.Invoke();
        else if (Button.interactable) OnButtonClick?.Invoke();
    }
}
