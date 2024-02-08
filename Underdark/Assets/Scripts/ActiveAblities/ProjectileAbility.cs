using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbility : ActiveAbility, IAttacker
{
    [Header("Projectile Settings")]
    [SerializeField] protected float projSpeed;
    public Transform Transform => transform;

    protected Rigidbody2D rb;
    protected Collider2D coll;
    

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        Invoke(nameof(Die),  AttackDistance.GetValue(abilityLevel) / projSpeed);
    }

    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);

        damageInfo.AddDamage(
            (int)Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel),
                MaxValue.GetValue(abilityLevel)), damageType, caster.Params.GetDamageAmplification(damageType));
        
        rb.velocity = attackDir * projSpeed;
        
        var rotAngle = Vector2.Angle(Vector3.up, rb.velocity);
        if (rb.velocity.x > 0) rotAngle *= -1;
        transform.Rotate(Vector3.forward, rotAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(caster.AttackMask == (caster.AttackMask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                Attack(damageable);
            }
            Die();
        }
    }

    protected virtual void Die()
    {
        coll.enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }
    
    public void Attack()
    {
        throw new NotImplementedException();
    }

    public void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, this, damageInfo))
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && distToTarget > 2;
    }
}
