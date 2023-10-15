using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveAblity : MonoBehaviour
{
    
    public virtual void Execute()
    {
        Instantiate(gameObject, transform.position, Quaternion.identity);
    }
}
