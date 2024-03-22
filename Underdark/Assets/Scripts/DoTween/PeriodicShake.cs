using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PeriodicShake : MonoBehaviour
{
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float shakeDelay = 1f;
    [SerializeField] private float shakeDuration = 0.2f;

    [Header("Strength")]
    [SerializeField] private float shakePositionStrength = 0.5f;
    [SerializeField] private float shakeRotationStrength = 90f;
    [SerializeField] private float shakeScaleStrength = 0.5f;
    
    private void OnEnable()
    {
        StartCoroutine(DoPeriodicShake());
    }

    private IEnumerator DoPeriodicShake()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            transform.DOShakePosition(shakeDuration, shakePositionStrength);
            transform.DOShakeRotation(shakeDuration, shakeRotationStrength);
            transform.DOShakeScale(shakeDuration, shakeScaleStrength);
            
            yield return new WaitForSeconds(shakeDelay);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.DOKill();
    }
}
