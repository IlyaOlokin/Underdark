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
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        characterWindowUI.Init(player);
        characterWindowUI.blackOut = blackOut;
    }

    private void Start()
    {
        TryCloseWindow();
    }

    private void OnEnable()
    {
        characterWindowUI.OnStatsApplied += TryCloseWindow;
    }
    
    private void TryCloseWindow()
    {
        if (characterWindowUI.Stats.FreePoints == 0)
            Destroy(gameObject);
    }
    
    private void OnDisable()
    {
        characterWindowUI.OnStatsApplied -= TryCloseWindow;

    }
}
