using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ActiveAblity : MonoBehaviour
{
    public float CastTime;
    public float cooldown;
    [SerializeField] protected LayerMask attackMask;
    [SerializeField] protected int attackDistance;
    [SerializeField] protected float attackAngle;
    [SerializeField] protected float maxDamage;
    [SerializeField] protected int damageStatMultiplier;
    
    protected float damage;

    [SerializeField] private bool needAutoDestroy;
    [SerializeField] private float autoDestroyDelay;
    
    protected Unit caster;

    private void Awake()
    {
        if (needAutoDestroy) Destroy(gameObject, autoDestroyDelay);
    }

    public virtual void Execute(Unit caster)
    {
        
    }
    
    protected Collider2D FindClosestTarget(Unit caster)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, attackDistance + 0.5f, contactFilter, hitColliders);

        Collider2D target = null;
        float minDist = float.MaxValue;
        foreach (var collider in hitColliders)
        {
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, caster.GetAttackDirection());
            if (angle < attackAngle / 2f && dir.magnitude < minDist)
            {
                minDist = dir.magnitude;
                target = collider;
            }
        }

        return target;
    }
    
    protected List<Collider2D> FindAllTargets(Unit caster)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, attackDistance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, caster.GetAttackDirection());
            if (angle < attackAngle / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }

    protected void OverrideWeaponStats(MeleeWeapon weapon)
    {
        if (weapon.ID == "empty") return;
        attackDistance = weapon.AttackDistance;
        attackAngle = weapon.AttackRadius;
    }
}
