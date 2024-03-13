using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StartCharacterWindow : MonoBehaviour
{
    [SerializeField] private CharacterWindowUI characterWindowUI;
    [SerializeField] private GameObject blackOut;
    private Player player;
    private IInput input;
    
    [Inject]
    private void Construct(Player player, IInput input)
    {
        this.player = player;
        characterWindowUI.Init(player);
        characterWindowUI.blackOut = blackOut;
        this.input = input;
    }

    private void Start()
    {
        input.IsEnabled = false;
        TryCloseWindow();
    }

    private void OnEnable()
    {
        characterWindowUI.OnStatsApplied += TryCloseWindow;
    }
    
    private void TryCloseWindow()
    {
        if (characterWindowUI.Stats.FreePoints != 0) return;
        
        input.IsEnabled = true;
        Destroy(gameObject);
    }
    
    private void OnDisable()
    {
        characterWindowUI.OnStatsApplied -= TryCloseWindow;

    }
}
