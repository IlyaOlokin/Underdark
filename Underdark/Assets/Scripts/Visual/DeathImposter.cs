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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }


    public void StartDeath(IAttacker attacker, DamageType damageType, Sprite sprite)
    {
        StartCoroutine(StartDissolve());
        rb.velocity = (transform.position - attacker.Transform.position).normalized * pushForce;
        sr.sprite = sprite;
    }

    IEnumerator StartDissolve()
    {
        yield return new WaitForSeconds(dissolveDuration);
        Destroy(gameObject);
    }
}
