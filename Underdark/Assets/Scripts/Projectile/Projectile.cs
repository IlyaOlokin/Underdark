using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IAttackerTarget
{
    public Transform Transform => transform;
    
    protected Unit caster;
    
    protected DamageInfo damageInfo = new();
    protected List<DebuffInfo> debuffInfos;
    protected int abilityLevel;
    protected int penetrationCount;
    protected List<IDamageable> damageablesToIgnore;
    
    protected Rigidbody2D rb;
    protected Collider2D coll;
    protected void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        
    }

    public void Init(Unit caster, DamageInfo damageInfo, List<DebuffInfo> debuffInfos, int abilityLevel, 
        Vector2 velocity, float destroyDelay, int penetrationCount, List<IDamageable> damageablesToIgnore)
    {
        this.caster = caster;
        
        Invoke(nameof(DieOld),  destroyDelay);
        this.damageInfo = damageInfo;
        this.debuffInfos = debuffInfos;
        this.abilityLevel = abilityLevel;
        this.penetrationCount = penetrationCount;
        this.damageablesToIgnore = damageablesToIgnore;
        
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
                if (damageablesToIgnore != null && damageablesToIgnore.Contains(damageable)) return;
                
                Attack(damageable);
            }
            else
            {
                Die(null);
                return;
            }
            
            if (penetrationCount == -1) return;
            
            if (penetrationCount > 0)
                penetrationCount--;
            else
                Die(damageable);
        }
    }

    public virtual void Attack(IDamageable damageable)
    {
        if (damageable.TakeDamage(caster, this, damageInfo))
        {
            foreach (var debuffInfo in debuffInfos)
            {
                debuffInfo.Execute(this, (Unit) damageable, caster);
            }
        }
    }
    
    protected virtual void Die(IDamageable damageable)
    {
        coll.enabled = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    protected void DieOld()
    {
        Die(null);
    }
}
