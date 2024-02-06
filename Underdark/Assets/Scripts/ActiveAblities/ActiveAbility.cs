using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class ActiveAbility : MonoBehaviour
{
    [field:SerializeField] public string ID { get; private set; }
    public ActiveAbilityLevelSetupSO ActiveAbilityLevelSetupSO;
    public float CastTime;
    [field:SerializeField] public ActiveAbilityProperty<float> Cooldown { get; private set; }
    [SerializeField] private ActiveAbilityProperty<int> manaCost;
    
    [field:SerializeField] public bool NeedOverrideWithWeaponStats { get; private set; }
    [field:SerializeField] public bool NeedAttackRadius { get; private set; }
    [field:SerializeField] public float AttackDistance { get; protected set; }
    [field:SerializeField] public float AttackRadius { get; protected set; }
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
    protected Vector2 attackDir;
    protected int abilityLevel;

    protected virtual void Awake()
    {
        if (needAutoDestroy) Destroy(gameObject, autoDestroyDelay);
    }

    public virtual void Execute(Unit caster, int exp)
    {
        if (NeedOverrideWithWeaponStats) 
            OverrideWeaponStats(caster.GetWeapon());
        this.caster = caster;
        abilityLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(exp);
        attackDir = caster.GetAttackDirection(AttackDistance);
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
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < AttackRadius / 2f && dir.magnitude < minDist)
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
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < AttackRadius / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }
    
    private void OverrideWeaponStats(WeaponSO weapon)
    {
        if (weapon.ID == "empty") return;
        AttackDistance = weapon.AttackDistance;
        AttackRadius = weapon.AttackRadius;
    }

    public bool RequirementsMet(WeaponSO weapon)
    {
        return validWeaponTypes.Contains(WeaponType.Any) || validWeaponTypes.Contains(weapon.WeaponType) || validWeaponTypes.Count == 0;
    }

    public int GetManaCost(int exp)
    {
        return manaCost.GetValue(ActiveAbilityLevelSetupSO.GetCurrentLevel(exp));
    }

    public virtual bool CanUseAbility(Unit caster, float distToTarget)
    {
        var attackDist = NeedOverrideWithWeaponStats ? caster.GetWeapon().AttackDistance : AttackDistance;
        return caster.CurrentMana >= GetManaCost(caster.GetExpOfActiveAbility(ID)) && (distToTarget <= attackDist + 0.7f || attackDist == 0);
    }

    public virtual string[] ToString(Unit owner)
    {
        var res = new string[7];
        var abilityLevel = GetManaCost(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        if (statMultiplier != 0) res[1] = $"Damage: {statMultiplier} * {UnitStats.GetStatString(baseStat)} (max: {maxValue})";
        if (abilityLevel != 0)       res[2] = $"Mana: {abilityLevel}";
        if (AttackDistance != 0) res[3] = $"Distance: {AttackDistance}";
        if (AttackRadius != 0 && NeedAttackRadius) res[4] = $"Radius: {AttackRadius}";
        if (Cooldown.GetValue(abilityLevel) != 0)    res[5] = $"Cooldown: {Cooldown.GetValue(abilityLevel)}";
        if (validWeaponTypes.Count != 0 && !validWeaponTypes.Contains(WeaponType.Any)) res[6] = $"Weapon: {GetValidWeaponTypesString()}";
        return res;
    }
    
    public string[] ToStringAdditional(Unit owner)
    {
        List<string> res = new List<string>();

        foreach (var debuffInfo in debuffInfos)
        {
            res.Add(debuffInfo.ToString());
        }

        return res.ToArray();
    }

    private string GetValidWeaponTypesString()
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < validWeaponTypes.Count; i++)
        {
            str.Append(validWeaponTypes[i]);
            if (i + 1 != validWeaponTypes.Count)
                str.Append(", ");
        }

        return str.ToString();
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
