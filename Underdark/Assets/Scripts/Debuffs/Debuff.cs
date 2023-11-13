using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff : MonoBehaviour, IStatusEffect
{
    [field:SerializeField] public Sprite Icon { get; protected set;}

    public float Duration { get; protected set;}
    public float Timer { get; protected set;}
    
    protected Unit receiver;
    protected Unit caster;
}
