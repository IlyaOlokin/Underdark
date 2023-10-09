using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalHealthBar : HealthBarController
{
    [SerializeField] private Unit damageable;

    protected virtual void Awake()
    {
        damageable.OnHealthChanged += UpdateHealth;
        damageable.OnMaxHealthChanged += SetMaxHP;
    }
}