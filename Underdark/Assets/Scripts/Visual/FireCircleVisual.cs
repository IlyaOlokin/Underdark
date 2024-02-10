using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCircleVisual : MonoBehaviour
{
    [SerializeField] private Transform particleSystem;

    private void Start()
    {
        StartVisualEffect(5f, 7f);
    }

    public void StartVisualEffect(float duration, float radius)
    {
        var endScale = radius * 2 + 1;
        var scaleSpeed = endScale / duration;
        
        Destroy(gameObject, duration);
        StartCoroutine(StartVisual(scaleSpeed));
    }
    
    IEnumerator StartVisual(float scaleSpeed)
    {
        while (true)
        {
            var newScale = particleSystem.localScale.x + scaleSpeed * Time.deltaTime;
            particleSystem.localScale = new Vector3(newScale, newScale);
            transform.localScale = new Vector3(newScale, newScale);
            
            yield return null;
        }
    }
}
