using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireBall : ActiveAblity
{
    [SerializeField] private float projSpeed;
    [SerializeField] private float maxDamage;
    [SerializeField] private int damageStatMultiplier;
    
    private int damage;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, attackDistance / projSpeed);
    }

    public override void Execute(Unit caster)
    {
        //damage = Mathf.Min(caster.Stats.Int * 2, maxDamage);
        damage = 2 * damageStatMultiplier;
        
        var target = FindClosestTarget(caster);

        if (target != null)
            rb.velocity = (target.transform.position - transform.position).normalized * projSpeed;
        else
            rb.velocity = caster.GetAttackDirection() * projSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackMask.value == 1 << other.gameObject.layer) // is layer in layer mask
        {
            other.GetComponent<IDamageable>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
