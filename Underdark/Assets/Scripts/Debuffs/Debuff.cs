using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff : MonoBehaviour
{
    public bool IsExpired => duration <= 0;

    protected float duration;

    public virtual void Update()
    {
        
    }
}
