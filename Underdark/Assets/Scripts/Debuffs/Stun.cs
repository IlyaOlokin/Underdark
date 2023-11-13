using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stun : Debuff
{
    private StunInfo stunInfo;
    private Unit receiver;
    private Slider stunBar;

    public void Init(StunInfo stunInfo, Unit receiver, GameObject stunBar, Sprite effectIcon)
    {
        Duration = stunInfo.Duration;
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
            receiver.GetUnStunned();
            stunBar.gameObject.SetActive(false);
            receiver.LooseBuff(this);

            Destroy(this);
        }
    }

    public void AddDuration(float addDuration)
    {
        Timer += addDuration;
        Duration += addDuration;
    }
}
