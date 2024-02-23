using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float delay;
    
    private void Awake()
    {
        Destroy(gameObject, delay);
    }
}
