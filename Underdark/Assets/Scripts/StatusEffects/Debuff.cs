using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff : MonoBehaviour, IStatusEffect
{
    public Sprite Icon { get; protected set;}
    public float Duration { get; protected set;}
    public float Timer { get; protected set;}
    
    protected Unit receiver;
    protected Unit caster;
}
