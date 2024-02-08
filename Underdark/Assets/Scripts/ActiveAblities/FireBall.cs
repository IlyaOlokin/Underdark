using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireBall : ProjectileAbility
{
    [Header("Visual")] 
    [SerializeField] private GameObject lightSpot;
    [SerializeField] private List<ParticleSystem> particleSystems;
    [SerializeField] private List<ParticleSystem> deathExplosion;
    [SerializeField] private float destroyDelay = 1.5f;

    protected override void Die()
    {
        coll.enabled = false;
        rb.velocity = Vector2.zero;
        lightSpot.SetActive(false);
        
        foreach (var system in particleSystems)
            system.Stop();
        
        foreach (var system in deathExplosion)
            system.Play();
        CancelInvoke(nameof(Die));
        Destroy(gameObject, destroyDelay);
    }
}
