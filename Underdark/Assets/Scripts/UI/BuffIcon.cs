using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    public IBuff Buff { get; private set; }

    [SerializeField] private Image icon;
    [SerializeField] private Image durationIndicator;

    private void Update()
    {
        durationIndicator.fillAmount = Buff.Timer / Buff.Duration;
    }

    public void SetData(IBuff buff)
    {
        Buff = buff;
        icon.sprite = buff.Icon;
    }
}
