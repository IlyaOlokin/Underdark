using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Unit : MonoBehaviour, IDamageable, IMover, IAttackerAOE, ICaster
{
    private Rigidbody2D rb;
    private Collider2D coll;
    protected Animator anim;
    public UnitStats Stats;
    public UnitParams Params;
    public Inventory Inventory;
    public EnergyShield EnergyShield;

    private bool isStunned;
    private bool isFrozen;
    protected bool IsDisabled => isStunned || isFrozen;
    protected bool IsPushing { get; private set; }
    public bool IsSilenced { get; private set; }
    public bool IsDead { get; private set; } 

    public event Action OnIsSilenceChanged;

    [SerializeField] private int baseMaxHP;
    public int MaxHP { get; private set; }

    private int _currentHP;

    public int CurrentHP
    {
        get => _currentHP;
        protected set
        {
            if (value > MaxHP) _currentHP = MaxHP;
            else if (value < 0) _currentHP = 0;
            else _currentHP = value;
            OnHealthChanged?.Invoke(_currentHP);
        }
    }

    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    public event Action OnUnitDeath;

    [SerializeField] private int baseMaxMana;
    public int MaxMana { get; private set; }

    private int _currentMana;

    public int CurrentMana
    {
        get => _currentMana;
        protected set
        {
            if (value > MaxMana) value = MaxMana;
            if (value < 0) value = 0;
            _currentMana = value;
            OnManaChanged?.Invoke(_currentMana);
        }
    }

    public event Action<int> OnManaChanged;
    public event Action<int> OnMaxManaChanged;
    
    [NonSerialized] public List<IStatusEffect> Buffs = new();
    public event Action<IStatusEffect> OnStatusEffectReceive;
    public event Action<IStatusEffect> OnStatusEffectLoose;
    
    private List<PassiveSO> Passives = new();
    
    public event Action OnUnitPassivesChanged;
    
    [field: SerializeField] public int MoveSpeed { get; private set; }

    [field: Header("Attack Setup")] 
    [SerializeField] private WeaponSO defaultWeapon;
    [SerializeField] private ActiveAbility baseAttackAbility;

    [SerializeField] private float attackSpeed;
    public event Action<float, float, float> OnBaseAttack;
    public event Action<int> OnExecutableItemUse;

    [field:SerializeField] public LayerMask AttackMask { get; protected set; }
    [field:SerializeField] public LayerMask AlliesLayer { get; protected set; }

    public Transform Transform => transform;

    [field: Header("Abilities Setup")]
    private string[] lastActiveAbilitiesIDs;
    
    [field: NonSerialized] public List<float> ActiveAbilitiesCD { get; private set; }
    public Dictionary<string, int> ActiveAbilitiesExp { get; private set; } = new Dictionary<string, int>();
    
    [Header("Inventory Setup")]
    [SerializeField] private int inventoryCapacity;
    [SerializeField] private int activeAbilityInventoryCapacity;
    
    [Header("Visual")] 
    [SerializeField] private GameObject visualsFlipable;
    [SerializeField] protected GameObject unitVisualRotatable;
    private bool facingRight = true;
    [SerializeField] protected UnitNotificationEffect unitNotificationEffect;
    [field:SerializeField] public UnitVisual UnitVisual { get; private set; }

    protected Vector3 lastMoveDir;
    protected float lastMoveDirAngle;
    protected float attackCDTimer;
    private float actionCDTimer;
    private float hpRegenBuffer;
    private float manaRegenBuffer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        
        lastMoveDir = Vector3.right;
        
        Inventory = new Inventory(inventoryCapacity, activeAbilityInventoryCapacity, this);
        
        ActiveAbilitiesCD = new List<float>(new float[Inventory.EquippedActiveAbilitySlots.Count]);
        lastActiveAbilitiesIDs = new string[Inventory.EquippedActiveAbilitySlots.Count];
        
        Params.SetUnit(this);
    }
    
    protected virtual void Start()
    {
        SetUnit();
    }

    protected void SetUnit()
    {
        IsDead = false;
        SetHP(true);
        SetMana(true);
        SetActiveAbilitiesCDs(true);
        GetUnStunned();
        EndPushState();
        LooseEnergyShield();
        StopAllCoroutines();
        foreach (var buffs in GetComponents<IStatusEffect>())
        {
            Destroy((Object)buffs);
        }
    }

    protected virtual void Update()
    {
        UpdateCoolDowns();
        Regeneration();
        RotateAttackDir();
    }
    
    protected abstract void RotateAttackDir();

    protected void SetMana(bool toFull = false)
    {
        float currentPart = CurrentMana / (float) MaxMana;
        MaxMana = baseMaxMana + Stats.GetTotalStatValue(BaseStat.Intelligence) * 10;
        OnMaxManaChanged?.Invoke(MaxMana); 

        if (toFull)
            CurrentMana = MaxMana;
        else
            CurrentMana = (int)Mathf.Floor(MaxMana * currentPart);
    }
    
    protected void UpdateMP()
    {
        SetMana(false);
    }

    protected void SetHP(bool toFull = false)
    {
        float currentPart = CurrentHP / (float) MaxHP;
        MaxHP = baseMaxHP + Stats.GetTotalStatValue(BaseStat.Strength) * 10;
        OnMaxHealthChanged?.Invoke(MaxHP);
        
        if (toFull)
            CurrentHP = MaxHP;
        else
            CurrentHP = (int)Mathf.Floor(MaxHP * currentPart);
    }

    protected void UpdateHP()
    {
        SetHP(false);
    }

    private void UpdateCoolDowns()
    {
        attackCDTimer -= Time.deltaTime;
        actionCDTimer -= Time.deltaTime;

        for (var index = 0; index < ActiveAbilitiesCD.Count; index++)
        {
            ActiveAbilitiesCD[index] -= Time.deltaTime * Params.SlowDebuffAmount * Params.CdSpeedMultiplier;
        }
    }

    private void Regeneration()
    {
        if (CurrentHP < MaxHP)
        {
            hpRegenBuffer += Params.GetRegenPerSecond(RegenType.HP) * Time.deltaTime;
            if (hpRegenBuffer >= 1)
            {
                RestoreHP((int)Mathf.Floor(hpRegenBuffer));
                hpRegenBuffer %= 1;
            }
        }
        else
        {
            hpRegenBuffer = 0;
        }

        if (CurrentMana < MaxMana)
        {
            manaRegenBuffer += Params.GetRegenPerSecond(RegenType.MP) * Time.deltaTime;
            if (manaRegenBuffer >= 1)
            {
                RestoreMP((int)Mathf.Floor(manaRegenBuffer));
                manaRegenBuffer %= 1;
            }
        }
        else
        {
            manaRegenBuffer = 0;
        }
    }
    
    protected void OnLevelUp()
    {
        SetHP(true);
        SetMana(true);
    }

    protected virtual bool TryFlipVisual(float moveDir)
    {
        if (moveDir < 0 && facingRight)
        {
            Flip();
            return true;
        } 
        if (moveDir > 0 && !facingRight)
        {
            Flip();
            return true;
        }
        return false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        visualsFlipable.transform.Rotate(0, 180, 0);
    }

    public virtual bool TakeDamage(Unit sender, IAttacker attacker, DamageInfo damageInfo, bool evadable = true, float armorPierce = 0f)
    {
        if (IsDead) return false;
        
        var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);

        if (evadable && TryToEvade())
        {
            newEffect.WriteMessage("Evaded!");
            return false;
        }

        var newDamage = CalculateTakenDamage(damageInfo, armorPierce);

        if (EnergyShield != null)
        {
            if (EnergyShield.TakeDamage(this, sender, attacker, newEffect, unitNotificationEffect, ref newDamage)) 
                return true;
        }
        
        CurrentHP -= newDamage;
        UnitVisual.StartWhiteOut();
        if (CurrentHP <= 0) Death(sender, attacker, damageInfo.GetDamages()[0].DamageType);
        newEffect.WriteDamage(newDamage);
        return true;
    }

    public void RestoreHP(int hp, bool showVisual = false)
    {
        CurrentHP += hp;
        if (showVisual)
        {
            var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);
            newEffect.WriteHeal(hp);
        }
    }
    
    public void RestoreMP(int mp)
    {
        CurrentMana += mp;
    }

    public void GetEnergyShield(int maxHP, float angle)
    {
        EnergyShield = new EnergyShield(maxHP, angle);
        UnitVisual.ActivateEnergyShieldVisual(angle);
    }
    
    public void LooseEnergyShield()
    {
        EnergyShield = null;
        UnitVisual.DeactivateEnergyShieldVisual();
    }
    
    public virtual void ApplySlowDebuff(float slow)
    {
        Params.ApplySlowDebuff(slow);
    }
    
    public void ResetAbilityCoolDowns()
    {
        for (int i = 0; i < ActiveAbilitiesCD.Count; i++)
        {
            if (Inventory.EquippedActiveAbilitySlots[i].IsEmpty) continue;
            var equippedActiveAbility = Inventory.GetEquippedActiveAbility(i);
            ActiveAbilitiesCD[i] = equippedActiveAbility.Cooldown.GetValue(GetExpOfActiveAbility(equippedActiveAbility.ID));
        }
    }

    public void InstantDeath(Unit killer, IAttacker attacker)
    {
        if (IsDead) return;
        
        var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);
        newEffect.WriteMessage("Crit!");
        Death(killer, attacker, DamageType.Chaos);
    }
    
    public virtual void GetStunned() => isStunned = true;

    public virtual void GetUnStunned() => isStunned = false;

    public virtual void GetFrozen() => isFrozen = true;

    public virtual void GetUnFrozen() => isFrozen = false;

    public void StartSilence()
    {
        IsSilenced = true;
        OnIsSilenceChanged?.Invoke();
    }
    
    public void EndSilence()
    {
        IsSilenced = false;
        OnIsSilenceChanged?.Invoke();
    }
    
    public virtual void GetPushed(Vector2 pushDir)
    {
        rb.velocity = pushDir;
    }

    public void StartPushState(bool isTrigger = false)
    {
        IsPushing = true;
        coll.isTrigger = isTrigger;
    }
    
    public virtual void EndPushState()
    {
        IsPushing = false;
        coll.isTrigger = false;
        rb.totalForce = Vector2.zero;
        
        if (transform.TryGetComponent(out Push pushComponent))
            Destroy(pushComponent);
    }

    public void ReceiveStatusEffect(IStatusEffect statusEffect)
    {
        Buffs.Add(statusEffect);
        OnStatusEffectReceive?.Invoke(statusEffect);
    }
    
    public void LooseStatusEffect(IStatusEffect statusEffect)
    {
        Buffs.Remove(statusEffect);
        OnStatusEffectLoose?.Invoke(statusEffect);
    }

    public void ReceivePassive(PassiveSO passive)
    {
        Passives.Add(passive);
        OnUnitPassivesChanged?.Invoke();
    }
    
    public void LoosePassive(PassiveSO passive)
    {
        Passives.Remove(passive);
        OnUnitPassivesChanged?.Invoke();
    }
    
    private int CalculateTakenDamage(DamageInfo damageInfo, float armorPierce)
    {
        int totalDamage = 0;
        
        for (int i = 0; i < damageInfo.GetDamages().Count; i++)
        {
            totalDamage += (int) Mathf.Floor(damageInfo.GetDamages()[i].GetValue() * Params.GetDamageResistance(damageInfo.GetDamages()[i].DamageType));
        }

        if (totalDamage == 0) return totalDamage;
        
        return Mathf.FloorToInt(totalDamage * (totalDamage / (totalDamage + GetTotalArmor() * (1 - armorPierce))));
    }

    public int GetTotalArmor()
    {
        var armor = GetArmorAmount(ItemType.Head) +
                    GetArmorAmount(ItemType.Body) +
                    GetArmorAmount(ItemType.Legs) + 
                    GetArmorAmount(ItemType.Shield);

        armor += Stats.GetTotalStatValue(BaseStat.Dexterity) / 2;
        return armor;
    }

    protected virtual void Death(Unit killer, IAttacker attacker, DamageType damageType)
    {
        IsDead = true;
        OnUnitDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public void Move(Vector3 dir)
    {
        if (IsDisabled || IsPushing) return;
        
        rb.MovePosition(rb.position + (Vector2)dir * (MoveSpeed * Params.SlowDebuffAmount * Params.MoveSpeedMultiplier) * Time.fixedDeltaTime);
        if (dir != Vector3.zero)
            lastMoveDir = dir;
        TryFlipVisual(dir.x);
    }
    
    public void GetMoved(Vector2 addVector)
    {
        rb.MovePosition(rb.position + addVector);
    }
    
    public virtual void Attack()
    {
        if (attackCDTimer > 0 || actionCDTimer > 0 || IsDisabled) return;
        
        var newBaseAttack = Instantiate(baseAttackAbility, transform.position, Quaternion.identity);
        newBaseAttack.Execute(this, 1, GetAttackDirection(newBaseAttack.AttackDistance.GetValue(1)));

        attackCDTimer = 1 / attackSpeed;
        SetActionCD(newBaseAttack.CastTime);

        OnBaseAttack?.Invoke(lastMoveDirAngle, GetWeapon().AttackRadius, GetWeapon().AttackDistance);
    }

    private bool TryToEvade()
    {
        return Random.Range(0f, 1f) < Params.GetEvasionChance();
    }

    public void ExecuteActiveAbility(int index)
    {
        if (actionCDTimer > 0 || ActiveAbilitiesCD[index] > 0 || IsDisabled || IsSilenced || Inventory.EquippedActiveAbilitySlots[index].IsEmpty) return;
        
        ActiveAbility activeAbility = Inventory.GetEquippedActiveAbility(index);
        var manaCost = activeAbility.GetManaCost(GetExpOfActiveAbility(activeAbility.ID));
        
        if (manaCost > CurrentMana) return;
        SpendMana(manaCost);
        
        var newAbility = Instantiate(activeAbility, transform.position, Quaternion.identity);
        var currentLevel = newAbility.ActiveAbilityLevelSetupSO.GetCurrentLevel(GetExpOfActiveAbility(activeAbility.ID));
        newAbility.Execute(this, currentLevel, GetAttackDirection(newAbility.AttackDistance.GetValue(currentLevel)));
        SetActionCD(newAbility.CastTime);
        ActiveAbilitiesCD[index] = newAbility.Cooldown.GetValue(GetExpOfActiveAbility(newAbility.ID));
    }

    protected void SetActiveAbilitiesCDs(bool reset = false)
    {
        for (int i = 0; i < Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            string newID = Inventory.EquippedActiveAbilitySlots[i].IsEmpty ? "" : Inventory.EquippedActiveAbilitySlots[i].ItemID;
            
            if (reset)
            {
                ActiveAbilitiesCD[i] = 0;
                lastActiveAbilitiesIDs[i] = newID;
                continue;
            }
            
            if (newID == "" && lastActiveAbilitiesIDs[i] != "")
            {
                ActiveAbilitiesCD[i] = 0;
                lastActiveAbilitiesIDs[i] = newID;
                continue;
            }
            if (newID != lastActiveAbilitiesIDs[i])
            {
                var equippedActiveAbility = Inventory.GetEquippedActiveAbility(i);
                ActiveAbilitiesCD[i] = equippedActiveAbility.Cooldown.GetValue(GetExpOfActiveAbility(equippedActiveAbility.ID));
            }

            lastActiveAbilitiesIDs[i] = newID;
        }
    }

    public void AddExpToActiveAbility(string abilityID, int exp)
    {
        if (ActiveAbilitiesExp.ContainsKey(abilityID))
        {
            ActiveAbilitiesExp[abilityID] += exp;
        }
        else
        {
            ActiveAbilitiesExp.Add(abilityID, exp);
        }
    }
    
    public int GetExpOfActiveAbility(string abilityID)
    {
        return ActiveAbilitiesExp.TryGetValue(abilityID, out var value) ? value : 0;
    }

    protected void ExecuteExecutableItem(int index)
    {
        if (IsDisabled || Inventory.ExecutableSlots[index].IsEmpty) return;

        if (!Inventory.GetExecutableItem(index).Execute(this)) return;
        
        Inventory.Remove(Inventory.ExecutableSlots[index]);
        OnExecutableItemUse?.Invoke(index);
    }

    public void SpendMana(int manaCost)
    {
        CurrentMana -= manaCost;
    }

    private void SetActionCD(float cd)
    {
        actionCDTimer = cd;
    }

    public List<T> GetAllPassives<T>()
    {
        var res = new List<T>();

        foreach (var passiveHolder in Inventory.GetAllPassiveHolders())
        {
            if (passiveHolder == null) continue;
            foreach (var passive in passiveHolder.Passives)
            {
                if (passive is T passive1)
                {
                    res.Add(passive1);
                }
            }
        }

        foreach (var passive in Passives)
        {
            if (passive is T passive1)
            {
                res.Add(passive1);
            }
        }

        return res;
    }
    
    public bool HasPassiveOfType<T>()
    {
        var passives = GetAllPassives<T>();
        return passives.Count != 0;
    }

    public WeaponSO GetWeapon()
    {
        if (Inventory.Equipment.Weapon.IsEmpty || !Inventory.Equipment.Weapon.IsValid)
            return defaultWeapon;
        return Inventory.Equipment.GetWeapon();
    }

    private int GetArmorAmount(ItemType itemType)
    {
        var armor = Inventory.Equipment.GetArmor(itemType);
        if (armor == null)
            return 0;

        return armor.ArmorAmount;
    }

    public virtual Vector2 GetAttackDirection(float distance = 0) => lastMoveDir.normalized;

    public virtual float GetAttackDirAngle(Vector2 attackDir = new Vector2()) => lastMoveDirAngle;

    public Vector2 GetLastMoveDir() => lastMoveDir.normalized;
}