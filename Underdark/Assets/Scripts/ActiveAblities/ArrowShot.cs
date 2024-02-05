using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShot : ActiveAbility, IAttacker
{
    [SerializeField] private float projSpeed;
    public Transform Transform => transform;
    
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, AttackDistance / projSpeed);
    }

    public override void Execute(Unit caster, int level)
    {
        base.Execute(caster, level);
        
        int damage = (int) Mathf.Min(caster.Stats.GetTotalStatValue(baseStat) * statMultiplier, maxValue);
        damageInfo.AddDamage(damage, multiplier: caster.Params.GetDamageAmplification(damageType));
        
        rb.velocity = attackDir * projSpeed;
        
        var rotAngle = Vector2.Angle(Vector3.up, rb.velocity);
        if (rb.velocity.x > 0) rotAngle *= -1;
        transform.Rotate(Vector3.forward, rotAngle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (caster.AttackMask == (caster.AttackMask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out Unit damageable))
            {
                Attack(damageable);
            }
            Destroy(gameObject);
        }
    }
    
    public void Attack()
    {
        
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
