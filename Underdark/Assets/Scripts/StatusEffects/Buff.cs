using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Buff : MonoBehaviour, IStatusEffect
{
    public Sprite Icon { get; protected set;}
    public float Duration { get; protected set;}
    public float Timer { get; protected set;}
    
    private Unit receiver;
    private PassiveSO passive;
    
    private void Init(Unit receiver, PassiveSO passive, float duration)
    {
        Duration = duration;
        Timer = duration;
        Icon = passive.Icon;
        this.passive = passive;
        this.receiver = receiver;
        
        receiver.ReceiveStatusEffect(this);
        receiver.ReceivePassive(passive);
    }
    
    private void Update()
    {
        Timer -= Time.deltaTime;
        
        if (Timer <= 0)
        {
            Destroy(this);
        }
    }
    
    private void OnDestroy()
    {
        receiver.LoosePassive(passive);
        receiver.LooseStatusEffect(this);
    }

    public static void ApplyBuff(Unit receiver, PassiveSO passive, float duration)
    {
        var newBuff = receiver.transform.AddComponent<Buff>();
        newBuff.Init(receiver, passive, duration);
    }

    
}
