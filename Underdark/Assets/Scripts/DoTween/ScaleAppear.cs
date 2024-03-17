using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleAppear : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float startScaleMultiplier;
    [SerializeField] private Ease ease = Ease.OutBack;
    
    void OnEnable()
    {
        var startScale = transform.localScale;
        transform.localScale *= startScaleMultiplier;
        transform.DOScale(startScale, duration).SetEase(ease);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
