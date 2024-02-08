using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IAttacker
{
    public Transform Transform => transform;
    
    private Unit caster;
    
    protected DamageInfo damageInfo = new();
    
    protected List<DebuffInfo> debuffInfos;
    
    protected Rigidbody2D rb;
    protected Collider2D coll;
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        
    }

    public void Init(Unit caster, float destroyDelay, DamageInfo damageInfo, List<DebuffInfo> debuffInfos, Vector2 velocity)
    {
        this.caster = caster;
        
        Invoke(nameof(Die),  destroyDelay);
        this.damageInfo = damageInfo;
        this.debuffInfos = debuffInfos;
        
        rb.velocity = velocity;
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
    
    protected virtual void Die()
    {
        coll.enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }
}
