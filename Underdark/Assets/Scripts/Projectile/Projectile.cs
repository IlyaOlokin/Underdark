using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IAttackerTarget
{
    public Transform Transform => transform;
    
    [SerializeField] protected float destroyDelay;
    [SerializeField] protected Collider2D ricochetCollider;
    
    protected Unit caster;
    
    protected DamageInfo damageInfo = new();
    protected List<DebuffInfo> debuffInfos;
    protected int abilityLevel;
    protected int penetrationCount;
    protected bool ableToRicochet;
    protected List<IDamageable> damageablesToIgnore;
    
    protected Rigidbody2D rb;
    protected Collider2D coll;

    public event Action OnCreate;
    public event Action OnDeath;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    protected void Start()
    {
        OnCreate?.Invoke();
    }

    public void Init(Unit caster, DamageInfo damageInfo, List<DebuffInfo> debuffInfos, int abilityLevel, 
        Vector2 velocity, float destroyDelay, int penetrationCount, bool ableToRicochet, List<IDamageable> damageablesToIgnore)
    {
        this.caster = caster;
        
        Invoke(nameof(DieOld),  destroyDelay);
        this.damageInfo = damageInfo;
        this.debuffInfos = debuffInfos;
        this.abilityLevel = abilityLevel;
        this.penetrationCount = penetrationCount;
        this.ableToRicochet = ableToRicochet;
        this.damageablesToIgnore = damageablesToIgnore;
        
        if (this.ableToRicochet) ricochetCollider.gameObject.SetActive(true);
        rb.velocity = velocity;
        SetRotation();
    }

    private void Update()
    {
        if (ableToRicochet) SetRotation();
    }

    private void SetRotation()
    {
        Vector2 v = rb.velocity;
        var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
                if (!ableToRicochet)
                    Die(null);
                return;
            }
            
            if (penetrationCount <= -1) return;
            
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

    protected void OnProjDeath()
    {
        OnDeath?.Invoke();
    }
}
