using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShieldAbility : ActiveAbility
{
    private int maxHP;
    private int currentHP;

    private float shieldRadius;
    
    [Header("Visual")] 
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private SpriteRenderer energyShieldVisual;
    [SerializeField] private float energyFillDuration;

    private static readonly int Turn = Shader.PropertyToID("_Turn");
    private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");

    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);
        caster.ParentToRotatable(transform);

        var shieldHp = caster.Stats.GetTotalStatValue(baseStat) * StatMultiplier.GetValue(abilityLevel);
        var maxValue = MaxValue.GetValue(abilityLevel);
        if (maxValue > 0) shieldHp = (int) Mathf.Min(shieldHp, maxValue);
        
        maxHP = shieldHp;
        currentHP = maxHP;
        shieldRadius = AttackAngle.GetValue(abilityLevel);
        
        caster.GetEnergyShield(this);

        ActivateEnergyShieldVisual(shieldRadius);
    }
    
    public bool TakeDamage(Unit owner, Unit sender, IAttacker attacker, UnitNotificationEffect newEffect,
        UnitNotificationEffect unitNotificationEffect, ref int newDamage)
    {
        var hasAttacker = attacker != null;
        Vector3 dir = hasAttacker ? attacker.Transform.position - owner.transform.position : sender.transform.position;
        var angle = Vector2.Angle(dir, owner.GetLastMoveDir());

        var savedDamage = newDamage;

        if (AbsorbDamage(ref newDamage, angle))
        {
            newEffect.WriteDamage(savedDamage, true);

            if (hasAttacker)
            {
                float exactAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + hitParticles.transform.eulerAngles.z;
                Instantiate(hitParticles, transform.position + dir.normalized / 2f, Quaternion.Euler(0, 0, exactAngle));
            }

            if (newDamage > 0)
            {
                var newEffectForES = Instantiate(unitNotificationEffect, owner.transform.position, Quaternion.identity);
                newEffectForES.WriteDamage(savedDamage - newDamage, true);
                owner.LooseEnergyShield();
                Destroy(gameObject);
            }
            else
                return true;
        }

        return false;
    }

    private bool AbsorbDamage(ref int damage, float angleToAttacker)
    {
        if (!(angleToAttacker <= shieldRadius / 2f)) return false;

        if (damage > currentHP)
            damage -= currentHP;
        else
        {
            currentHP -= damage;
            damage = 0;
        }
        
        return true;
    }
    
    private void ActivateEnergyShieldVisual(float radius)
    {
        energyShieldVisual.material = new Material(energyShieldVisual.material);
        
        energyShieldVisual.material.SetFloat(Turn, 90);
        StartCoroutine(FillEnergyShield(radius));
    }

    private IEnumerator FillEnergyShield(float radius)
    {
        var progress = 0f;
        while (progress < 1)
        {
            progress += Time.deltaTime / energyFillDuration;
            var newRadius = Mathf.Lerp(0, radius, progress);
            energyShieldVisual.material.SetFloat(FillAmount, newRadius);
            yield return null;
        }
    }
    
    public override bool CanUseAbility(Unit caster, float distToTarget)
    {
        return base.CanUseAbility(caster, distToTarget) && caster.CurrentHP < caster.MaxHP * 0.5f;
    }
    
    public override string[] ToString(Unit owner)
    {
        var res = new string[5];
        var currentLevel = ActiveAbilityLevelSetupSO.GetCurrentLevel(owner.GetExpOfActiveAbility(ID));

        res[0] = description;
        if (StatMultiplier.GetValue(currentLevel) != 0)
            res[1] =
                $"Shield durability: {StatMultiplier.GetValue(currentLevel)} * {UnitStats.GetStatString(baseStat)}" +
                MaxValueToString(currentLevel);
        if (GetManaCost(owner.GetExpOfActiveAbility(ID)) != 0) res[2] = $"Mana: {GetManaCost(owner.GetExpOfActiveAbility(ID))}"; 
        res[3] = $"Shield Radius: {AttackAngle.GetValue(currentLevel)}";
        return res;
    }
}