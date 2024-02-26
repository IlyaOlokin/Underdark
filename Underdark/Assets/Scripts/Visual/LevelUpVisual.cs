using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LevelUpVisual : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Sequence sequence;
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        
        text = GetComponent<TextMeshProUGUI>();
        OnAnimEnd();
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
        if (sequence != null && sequence.IsPlaying()) return;
        text.enabled = true;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(40, 0.8f).SetEase(Ease.OutBack));
        sequence.Append(transform.DOLocalMoveY(-44, 0.8f).SetDelay(2f).SetEase(Ease.InBack));
        sequence.OnComplete(OnAnimEnd);
    }
    
    private void OnAnimEnd()
    {
        text.enabled = false;
        sequence.Kill();
        sequence = null;
    }
}