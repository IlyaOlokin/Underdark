using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementalIconUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    [SerializeField] private Sprite physicIcon;
    [SerializeField] private Sprite chaosIcon;
    [SerializeField] private Sprite fireIcon;
    [SerializeField] private Sprite airIcon;
    [SerializeField] private Sprite waterIcon;
    [SerializeField] private Sprite coldIcon;
    [SerializeField] private Sprite electricIcon;

    public void Init(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physic:
                icon.sprite = physicIcon;
                break;
            case DamageType.Chaos:
                icon.sprite = chaosIcon;
                break;
            case DamageType.Fire:
                icon.sprite = fireIcon;
                break;
            case DamageType.Cold:
                icon.sprite = coldIcon;
                break;
            case DamageType.Electric:
                icon.sprite = electricIcon;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
        }
    }
}
