using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stun : Debuff
{
    private float totalDuration;
    private StunInfo stunInfo;
    private IStunable receiver;
    private Slider stunBar;

    public void Init(StunInfo stunInfo, IStunable receiver, GameObject stunBar)
    {
        duration = stunInfo.Duration;
        totalDuration = stunInfo.Duration;
        this.receiver = receiver;
        this.stunBar = stunBar.GetComponent<Slider>();
        this.stunBar.gameObject.SetActive(true);
    }
    
    void Update()
    {
        duration -= Time.deltaTime;
        stunBar.value = duration / totalDuration;
        if (duration <= 0)
        {
            receiver.GetUnStunned();
            stunBar.gameObject.SetActive(false);
            Destroy(this);
        }
    }

    public void AddDuration(float addDuration)
    {
        duration += addDuration;
        totalDuration += addDuration;
    }
}
