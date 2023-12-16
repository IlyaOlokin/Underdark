using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DeathImposter : MonoBehaviour
{
    [SerializeField] private float pushForce;

    [Header("Dissolve")]
    [SerializeField] private float dissolveDuration;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    private static readonly int Fade = Shader.PropertyToID("_Fade");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }


    public void StartDeath(IAttacker attacker, DamageType damageType, Sprite sprite)
    {
        StartCoroutine(StartDissolve());
        sr.sprite = sprite;

        if (attacker == null) return;
        
        rb.velocity = (transform.position - attacker.Transform.position).normalized * pushForce; 
    }

    IEnumerator StartDissolve()
    {
        for (var alpha = dissolveDuration; alpha >= 0; alpha -= Time.deltaTime)
        {
            sr.material.SetFloat(Fade, alpha / dissolveDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
