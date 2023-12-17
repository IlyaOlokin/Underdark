using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ActiveAbility : MonoBehaviour
{
    public float CastTime;
    public float cooldown;
    public int ManaCost;
    //[SerializeField] protected LayerMask attackMask;
    [field:SerializeField] public bool NeedOverrideWithWeaponStats { get; private set; }
    [field:SerializeField] public float AttackDistance { get; protected set; }
    [field:SerializeField] public float AttackAngle { get; protected set; }
    [SerializeField] protected float maxValue;
    [SerializeField] protected int statMultiplier;
    [SerializeField] protected BaseStat baseStat;
    [SerializeField] protected DamageType damageType;
    [SerializeField] protected List<WeaponType> validWeaponTypes;
    
    protected DamageInfo damageInfo = new();
    
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

    public virtual void Execute(Unit caster)
    {
        if (NeedOverrideWithWeaponStats) 
            OverrideWeaponStats(caster.GetWeapon());
        this.caster = caster;
    }
    
    protected Collider2D FindClosestTarget(Unit caster)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, AttackDistance + 0.5f, contactFilter, hitColliders);

        Collider2D target = null;
        float minDist = float.MaxValue;
        foreach (var collider in hitColliders)
        {
            if (!HitCheck(caster.transform,collider.transform, contactFilter)) continue;

            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, caster.GetAttackDirection());
            if (angle < AttackAngle / 2f && dir.magnitude < minDist)
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
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, AttackDistance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            if (!HitCheck(caster.transform,collider.transform, contactFilter)) continue;
            
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, caster.GetAttackDirection());
            if (angle < AttackAngle / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }
    
    private void OverrideWeaponStats(MeleeWeapon weapon)
    {
        if (weapon.ID == "empty") return;
        AttackDistance = weapon.AttackDistance;
        AttackAngle = weapon.AttackRadius;
    }

    public bool RequirementsMet(MeleeWeapon weapon)
    {
        return validWeaponTypes.Contains(WeaponType.Any) || validWeaponTypes.Contains(weapon.WeaponType) || validWeaponTypes.Count == 0;
    }

    public new virtual string[] ToString()
    {
        var res = new string[6];
        res[0] = description;
        if (statMultiplier != 0) res[1] = $"Damage: {statMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {maxValue})";
        if (ManaCost != 0)       res[2] = $"Mana: {ManaCost}";
        if (AttackDistance != 0) res[3] = $"Distance: {AttackDistance}";
        if (AttackAngle != 0)    res[4] = $"Radius: {AttackAngle}";
        if (cooldown != 0)    res[5] = $"Cooldown: {cooldown}";
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
    
    public static bool HitCheck(Transform caster, Transform target, ContactFilter2D contactFilter)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        Physics2D.Raycast(caster.transform.position,
            target.position - caster.transform.position,
            contactFilter,
            hits);
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Wall")) return false;
            if (hit.transform == target) return true;
        }
        
        return true;
    }
}
