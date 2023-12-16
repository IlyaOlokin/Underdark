using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameUiWindow : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    
    protected Player player;
    private IInput input;

    [Inject]
    protected virtual void Construct(Player player, IInput input)
    {
        this.input = input;
        this.player = player;
    }

    protected virtual void Awake()
    {
        closeButton.onClick.AddListener(CloseWindow);
    }

    protected virtual void CloseWindow()
    {
        input.IsEnabled = true;
        gameObject.SetActive(false);
    }
    
    public virtual void OpenWindow()
    {
        input.IsEnabled = false;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
}
