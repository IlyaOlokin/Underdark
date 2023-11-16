using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Freeze : Debuff
{
    private FreezeInfo freezeInfo;
    private Slider stunBar;

    public void Init(FreezeInfo freezeInfo, Unit receiver, GameObject stunBar, Sprite effectIcon)
    {
        Duration = freezeInfo.Duration;
        Timer = Duration;
        this.receiver = receiver;
        Icon = effectIcon;
        this.stunBar = stunBar.GetComponent<Slider>();
        this.stunBar.gameObject.SetActive(true);
    }
    
    void Update()
    {
        Timer -= Time.deltaTime;
        stunBar.value = Timer / Duration;
        if (Timer <= 0)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        stunBar.gameObject.SetActive(false);
        receiver.GetUnFrozen();
        receiver.LooseStatusEffect(this);
    }

    public void AddDuration(float addDuration)
    {
        Timer += addDuration;
        Duration += addDuration;
    }
}
