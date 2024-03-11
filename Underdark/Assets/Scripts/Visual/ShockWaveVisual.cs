using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveVisual : MonoBehaviour
{
    private float effectDuration;
    [SerializeField] private float startValue;
    [SerializeField] private float endValue;
    
    private Material material;
    private static readonly int WaveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void StartVisual(float effectDuration, float attackDist)
    {
        this.effectDuration = effectDuration;
        transform.localScale = Vector3.one * (attackDist * 2 + 1);
        StartCoroutine(ShockWaveEffect());
    }

    private IEnumerator ShockWaveEffect()
    {
        material.SetFloat(WaveDistanceFromCenter, startValue);

        float progress = 0f;
        float timer = 0f;

        while (timer < effectDuration)
        {
            timer += Time.deltaTime;

            progress = Mathf.Lerp(startValue, endValue, timer / effectDuration);
            material.SetFloat(WaveDistanceFromCenter, progress);
            yield return null;
        }
    }
}
