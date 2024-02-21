using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialFillVisual : MonoBehaviour
{
    private SpriteRenderer sr;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public IEnumerator StartVisual(float attackDist, float attackDirAngle, float attackAngle, float duration, float scaleLerpSpeed)
    {
        sr.material = new Material(sr.material);
        
        var targetScale = transform.localScale * (attackDist * 2 + 1);
        transform.localScale = Vector3.zero;
        
        sr.material.SetFloat("_Turn", attackDirAngle);
        sr.material.SetFloat("_FillAmount", attackAngle);
        
        while (duration > 0)
        {
            transform.localScale = Vector3.Lerp( transform.localScale, targetScale, scaleLerpSpeed / duration * Time.deltaTime);
            duration -= Time.deltaTime;
            yield return null;
        }
    }
}
