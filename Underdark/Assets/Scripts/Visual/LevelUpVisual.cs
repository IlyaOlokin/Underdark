using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator), typeof(TextMeshProUGUI))]
public class LevelUpVisual : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Animator anim;
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        anim = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
        DisableText();
    }

    private void OnEnable()
    {
        player.Stats.OnLevelUp += StartVisual;
    }

    private void OnDisable()
    {
        player.Stats.OnLevelUp -= StartVisual;
    }

    private void StartVisual()
    {
        anim.SetTrigger("LevelUp");
    }

    private void EnableText()
    {
        text.enabled = true;
    }
    
    private void DisableText()
    {
        text.enabled = false;
    }
}