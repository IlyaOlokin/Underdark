using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Firefly : ActiveAbility
{
    private Vector3 currentTargetDir;
    
    [SerializeField] private float distanceFromCaster;
    [SerializeField]private float speed;
    
    [SerializeField] private float changePositionDelay;
    private float changePositionTimer;
    
    [Header("Visual")] 
    [SerializeField] private LightSourceVisual lightSourceVisual;

    protected override void Awake()
    {
        base.Awake();
        lightSourceVisual.LightUp();
    }
    
    private void Update()
    {
        changePositionTimer -= Time.deltaTime;
        if (changePositionTimer <= 0)
        {
            SetNewTargetPosition();
            changePositionTimer = changePositionDelay;
        }

        transform.position = Vector3.Lerp(transform.position,
            caster.transform.position + (currentTargetDir * distanceFromCaster), speed * Time.deltaTime);
    }

    private void SetNewTargetPosition()
    {
        currentTargetDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnDestroy()
    {
        lightSourceVisual.LightDown();
    }
}
