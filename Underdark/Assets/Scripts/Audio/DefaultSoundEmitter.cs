using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultSoundEmitter : MonoBehaviour, ISoundEmitterOnCreate, ISoundEmitterOnDeath
{
    public event Action OnCreateSound;
    public event Action OnDeathSound;

    private void Start()
    {
        OnCreateSound?.Invoke();
    }
    
    private void OnDisable()
    {
        OnDeathSound?.Invoke();
    }
}
