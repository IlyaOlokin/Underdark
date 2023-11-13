using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{
    public IStatusEffect StatusEffect { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private Image durationIndicator;

    [SerializeField] private Color buffColor;
    [SerializeField] private Color debuffColor;

    private void Update()
    {
        durationIndicator.fillAmount = StatusEffect.Timer / StatusEffect.Duration;
    }

    public void SetData(IStatusEffect statusEffect)
    {
        StatusEffect = statusEffect;
        icon.sprite = statusEffect.Icon;

        durationIndicator.color = statusEffect switch
        {
            Debuff debuff => debuffColor,
            _ => buffColor
        };
    }
}
