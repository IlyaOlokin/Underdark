using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalHealthBar : BarController
{
    [SerializeField] private Unit damageable;
    [SerializeField] private Image fill;
    [SerializeField] private Color enemyColor;
    [SerializeField] private Color playerColor;

    protected virtual void Awake()
    {
        damageable.OnHealthChanged += UpdateValue;
        damageable.OnMaxHealthChanged += SetMaxValue;
    }

    private void Start()
    {
        SetFillColor();
    }

    private void SetFillColor()
    {
        if (damageable.CompareTag("Player"))
        {
            fill.color = playerColor;
        }
        else if (damageable.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
            fill.color = enemyColor;
        }
    }
}