using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class ActiveAbility : MonoBehaviour
{
    [field:SerializeField] public string ID { get; private set; }
    public ActiveAbilityLevelSetupSO ActiveAbilityLevelSetupSO;
    public float CastTime;
    [field:SerializeField] public ScalableProperty<float> Cooldown { get; private set; }
    [SerializeField] private ScalableProperty<int> manaCost;
    
    [field:SerializeField] public bool NeedAttackRadiusDisplay { get; private set; }
    [field:SerializeField] public ScalableProperty<float> AttackDistance { get; protected set; }
    [field:SerializeField] public ScalableProperty<float> AttackRadius { get; protected set; }
    [field:SerializeField] protected ScalableProperty<float> MaxValue { get; set; }
    [field:SerializeField] protected ScalableProperty<int> StatMultiplier { get; set; }
    [SerializeField] protected BaseStat baseStat;
    [SerializeField] protected DamageType damageType;
    [SerializeField] protected List<WeaponType> validWeaponTypes;
    
    protected DamageInfo damageInfo = new();
    
    [SerializeField] protected ScalableProperty<DebuffInfoList> debuffInfos;
    
    [SerializeField] private bool needAutoDestroy;
    [SerializeField] private float autoDestroyDelay;
    
    [Header("Description")] 
    [Multiline] [SerializeField] protected string description;
    
    protected Unit caster;
    protected Vector2 attackDir;
    protected int abilityLevel;
    protected List<IDamageable> damageablesToIgnore;

    protected virtual void Awake()
    {
        if (needAutoDestroy) Destroy(gameObject, autoDestroyDelay);
    }

    public virtual void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore = null)
    {
        this.caster = caster;
        abilityLevel = level;
        this.damageablesToIgnore = damageablesToIgnore;
        this.attackDir = attackDir;
    }
    
    protected Collider2D FindClosestTarget(Unit caster, Vector3 center, float distance, List<IDamageable> objectsToIgnore = null)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(center, distance + 0.5f, contactFilter, hitColliders);

        Collider2D target = null;
        float minDist = float.MaxValue;
        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable)) continue;
            if (objectsToIgnore != null && objectsToIgnore.Contains(damageable)) continue;
            if (!HitCheck(center, collider.transform, contactFilter)) continue;

            Vector3 dir = collider.transform.position - center;
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < AttackRadius.GetValue(abilityLevel) / 2f && dir.magnitude < minDist)
            {
                minDist = dir.magnitude;
                target = collider;
            }
        }

        return target;
    }
    
    protected List<Collider2D> FindAllTargets(Unit caster, Vector3 center, float distance, List<IDamageable> objectsToIgnore = null)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(caster.AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(center, distance + 0.5f, contactFilter, hitColliders);

        List<Collider2D> targets = new List<Collider2D>();
        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out IDamageable damageable)) continue;
            if (objectsToIgnore != null && objectsToIgnore.Contains(damageable)) continue;
            if (!HitCheck(center, collider.transform, contactFilter)) continue;
            
            Vector3 dir = collider.transform.position - center;
            var angle = Vector2.Angle(dir, attackDir);
            if (angle < AttackRadius.GetValue(abilityLevel) / 2f)
            {
                targets.Add(collider);
            }
        }

        return targets;
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
        var attackDist = AttackDistance.GetValue(abilityLevel);
        return caster.CurrentMana >= GetManaCost(caster.GetExpOfActiveAbility(ID)) && (distToTarget <= attackDist + 0.7f || attackDist == 0);
    }

    public virtual string[] ToString(Unit owner)
    {
        var res = new string[7];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        if (StatMultiplier.GetValue(currentLevel) != 0)
            res[1] =
                $"Damage: {StatMultiplier.GetValue(currentLevel)} * {UnitStats.GetStatString(baseStat)} (max: {MaxValue.GetValue(currentLevel)})";
        if (currentLevel != 0)       res[2] = $"Mana: {manaCost.GetValue(currentLevel)}";
        if (AttackDistance.GetValue(currentLevel) != 0) res[3] = $"Distance: {AttackDistance.GetValue(currentLevel)}";
        if (AttackRadius.GetValue(currentLevel) != 0 && NeedAttackRadiusDisplay) res[4] = $"Radius: {AttackRadius.GetValue(currentLevel)}";
        if (Cooldown.GetValue(currentLevel) != 0)    res[5] = $"Cooldown: {Cooldown.GetValue(currentLevel)}";
        if (validWeaponTypes.Count != 0 && !validWeaponTypes.Contains(WeaponType.Any)) res[6] = $"Weapon: {GetValidWeaponTypesString()}";
        return res;
    }
    
    public string[] ToStringAdditional(Unit owner)
    {
        List<string> res = new List<string>();
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        foreach (var debuffInfo in debuffInfos.GetValue(currentLevel).DebuffInfos)
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
    
    public static bool HitCheck(Vector3 startPos, Transform target, ContactFilter2D contactFilter)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        Physics2D.Raycast(startPos, target.position - startPos,
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
