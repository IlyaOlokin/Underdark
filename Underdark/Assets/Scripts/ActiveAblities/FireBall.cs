using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireBall : ActiveAblity
{
    [SerializeField] private float projSpeed;
    
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
        
        var rotAngle = Vector2.Angle(Vector3.up, rb.velocity);
        if (rb.velocity.x > 0) rotAngle *= -1;
        transform.Rotate(Vector3.forward, rotAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(attackMask == (attackMask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
