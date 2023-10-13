using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttackAlert : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] private float maxThickness;
    [SerializeField] private float duration;
    private float timer;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().material = new Material(mat);
        mat = GetComponent<SpriteRenderer>().material;
    }

    public void StartAlert()
    {
        StopAllCoroutines();
        StartCoroutine(Alert());
    }

    IEnumerator Alert()
    {
        float currentThickness = 0;
        
        float speed = maxThickness / duration;
        timer = duration;

        while (timer > 0)
        {
            currentThickness += speed * Time.deltaTime;
            timer -= Time.deltaTime;
            mat.SetFloat("_Thickness", currentThickness);
            yield return null;
        }
        mat.SetFloat("_Thickness", 0);
    }
}
