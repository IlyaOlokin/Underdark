using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireBall : ActiveAbility, IAttacker
{
    [SerializeField] private float projSpeed;
    public Transform Transform => transform;

    private Rigidbody2D rb;
    private Collider2D coll;

    [Header("Visual")] 
    [SerializeField] private GameObject lightSpot;
    [SerializeField] private List<ParticleSystem> particleSystems;
    [SerializeField] private List<ParticleSystem> deathExplosion;
    [SerializeField] private float destroyDelay = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        Invoke(nameof(Die),  AttackDistance / projSpeed);
    }

    public override void Execute(Unit caster)
    {
        this.caster = caster;
        damage = Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        
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
                Attack(damageable);
            }
            Die();
        }
    }

    private void Die()
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
    
    public void Attack()
    {
        throw new NotImplementedException();
    }

    public void Attack(IDamageable damageable)
    {
        damageable.TakeDamage(caster, this, damage);
    }
}
