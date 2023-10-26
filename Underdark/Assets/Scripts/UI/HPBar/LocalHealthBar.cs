using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalHealthBar : BarController
{
    [SerializeField] private Unit damageable;

    protected virtual void Awake()
    {
        damageable.OnHealthChanged += UpdateValue;
        damageable.OnMaxHealthChanged += SetMaxValue;
    }
}