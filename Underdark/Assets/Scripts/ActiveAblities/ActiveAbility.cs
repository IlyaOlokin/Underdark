using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ActiveAbility : MonoBehaviour
{
    public float CastTime;
    public float cooldown;
    public int ManaCost;
    [SerializeField] protected LayerMask attackMask;
    [SerializeField] protected int attackDistance;
    [SerializeField] protected float attackAngle;
    [SerializeField] protected float maxValue;
    [SerializeField] protected int statMultiplier;
    [SerializeField] protected BaseStat baseStat;
    
    protected float damage;
    
    [SerializeField] protected List<DebuffInfo> debuffInfos;
    
    [SerializeField] private bool needAutoDestroy;
    [SerializeField] private float autoDestroyDelay;
    
    [Header("Description")] 
    [Multiline] [SerializeField] protected string description;
    
    protected Unit caster;

    private void Awake()
    {
        if (needAutoDestroy) Destroy(gameObject, autoDestroyDelay);
    }

    public abstract void Execute(Unit caster);
    
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

    public new virtual string[] ToString()
    {
        var res = new string[5];
        res[0] = description;
        if (statMultiplier != 0) res[1] = $"Damage: {statMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {maxValue})";
        if (ManaCost != 0)       res[2] = $"Mana: {ManaCost}";
        if (attackDistance != 0) res[3] = $"Distance: {attackDistance}";
        if (attackAngle != 0)    res[4] = $"Radius: {attackAngle}";
        return res;
    }
    
    public string[] ToStringAdditional()
    {
        List<string> res = new List<string>();

        foreach (var debuffInfo in debuffInfos)
        {
            res.Add(debuffInfo.ToString());
        }

        return res.ToArray();
    }
}
