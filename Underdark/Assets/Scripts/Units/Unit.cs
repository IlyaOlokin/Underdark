using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker, ICaster
{
    private Rigidbody2D rb;
    private Collider2D coll;
    public UnitStats Stats;
    public UnitParams Params;
    public Inventory Inventory;
    public EnergyShield EnergyShield;

    private bool isStunned;
    private bool isFrozen;
    protected bool IsStunned => isStunned || isFrozen;
    protected bool IsPushing { get; private set; }
    public bool IsSilenced { get; private set; }
    public event Action OnIsSilenceChanged;

    [SerializeField] private int baseMaxHP;
    public int MaxHP { get; private set; }

    private int _currentHP;

    public int CurrentHP
    {
        get => _currentHP;
        protected set
        {
            if (value > MaxHP) value = MaxHP;
            _currentHP = value;
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

    [SerializeField] [Range(0f, 1f)] private float regenPercent;

    [field: SerializeField] public int MoveSpeed { get; private set; }

    [field: Header("Attack Setup")] [SerializeField]
    private MeleeWeapon defaultWeapon;
    [SerializeField] private ActiveAbility baseAttackAbility;

    [SerializeField] private float attackSpeed;
    public event Action<float, float, float> OnBaseAttack;
    public event Action<int> OnExecutableItemUse;

    [NonSerialized] public List<IStatusEffect> Buffs = new();
    public event Action<IStatusEffect> OnStatusEffectReceive;
    public event Action<IStatusEffect> OnStatusEffectLoose;

    [field:SerializeField] public LayerMask AttackMask { get; protected set; }
    public Transform Transform => transform;

    [field: Header("Abilities Setup")]
    private string[] lastActiveAbilitiesIDs;

    [field: NonSerialized] public List<float> ActiveAbilitiesCD { get; private set; }
    
    [Header("Inventory Setup")]
    [SerializeField] private int inventoryCapacity;
    [SerializeField] private int activeAbilityInventoryCapacity;
    
    [Header("Visual")] 
    [SerializeField] private GameObject visualsFlipable;
    [SerializeField] protected GameObject unitVisualRotatable;
    private bool facingRight = true;
    [SerializeField] protected UnitNotificationEffect unitNotificationEffect;
    [SerializeField] protected UnitVisual unitVisual;

    protected Vector3 lastMoveDir;
    protected float attackDirAngle;
    protected float attackCDTimer;
    private float actionCDTimer;
    private float hpRegenBuffer;
    private float manaRegenBuffer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        Params.SetUnit(this);
        
        Inventory = new Inventory(inventoryCapacity, activeAbilityInventoryCapacity, this);
        
        ActiveAbilitiesCD = new List<float>(new float[Inventory.EquippedActiveAbilitySlots.Count]);
        lastActiveAbilitiesIDs = new string[Inventory.EquippedActiveAbilitySlots.Count];
    }
    
    private void Start()
    {
        SetUnit();
    }

    protected void SetUnit()
    {
        SetHP(true);
        SetMana(true);
        SetActiveAbilitiesCDs(true);
        GetUnStunned();
        EndPush();
        LooseEnergyShield();
        foreach (var buffs in GetComponents<IStatusEffect>())
        {
            Destroy((Object)buffs);
        }
    }

    protected virtual void Update()
    {
        UpdateCoolDowns();
        Regeneration();
    }
    
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
            ActiveAbilitiesCD[index] -= Time.deltaTime / Params.SlowAmount;
        }
    }

    private void Regeneration()
    {
        if (CurrentHP < MaxHP)
        {
            hpRegenBuffer += MaxHP * regenPercent * Time.deltaTime;
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
            manaRegenBuffer += MaxMana * regenPercent * Time.deltaTime;
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

    protected void TryFlipVisual(float moveDir)
    {
        if (moveDir < 0 && facingRight)
            Flip();
        if (moveDir > 0 && !facingRight)
            Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        visualsFlipable.transform.Rotate(0, 180, 0);
    }

    public virtual bool TakeDamage(Unit sender, IAttacker attacker, DamageInfo damageInfo, bool evadable = true, float armorPierce = 0f)
    {
        var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);

        if (evadable && TryToEvade())
        {
            newEffect.WriteMessage("Evaded!");
            return false;
        }

        var newDamage = CalculateTakenDamage(damageInfo, armorPierce);

        if (EnergyShield != null)
        {
            Vector3 dir = attacker.Transform.position - transform.position;
            var angle = Vector2.Angle(dir, GetAttackDirection());
            
            var savedDamage = newDamage;

            if (EnergyShield.AbsorbDamage(ref newDamage, angle))
            {
                newEffect.WriteDamage(savedDamage, true);

                if (newDamage > 0)
                {
                    var newEffectForES = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);
                    newEffectForES.WriteDamage(savedDamage - newDamage, true);
                    LooseEnergyShield();
                }
                    
                else
                    return true;
            }
        }
        
        CurrentHP -= newDamage;
        unitVisual.StartWhiteOut();
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

    public void GetEnergyShield(int maxHP, float radius)
    {
        EnergyShield = new EnergyShield(maxHP, radius);
        unitVisual.ActivateEnergyShieldVisual(radius);
    }
    
    private void LooseEnergyShield()
    {
        EnergyShield = null;
        unitVisual.DeactivateEnergyShieldVisual();
    }

    public void GetHarmOverTime(HarmInfo harmInfo, Unit caster, GameObject visual, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > harmInfo.chance) return;
        
        var newPoison = gameObject.AddComponent<HarmOverTime>();
        newPoison.Init(harmInfo, this, caster, visual, effectIcon);
        ReceiveStatusEffect(newPoison);
    }
    
    public void GetSlowed(SlowInfo slowInfo, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > slowInfo.chance) return;

        var newSlow = transform.AddComponent<Slow>();
        newSlow.Init(slowInfo, this, effectIcon);
        ReceiveStatusEffect(newSlow);
    }
    
    public void GetBurn(BurnInfo burnInfo, Unit caster, GameObject visual, Sprite effectIcon)
    {
        if (TryGetComponent(out Freeze freeze))
            Destroy(freeze);
        
        if (Random.Range(0f, 1f) > burnInfo.chance) return;
        if (TryGetComponent(out Burn burn)) return;
        
        var newBurn = gameObject.AddComponent<Burn>();
        newBurn.Init(burnInfo, this, caster, visual, effectIcon);
        ReceiveStatusEffect(newBurn);
    }
    
    public virtual void ApplySlow(float slow)
    {
        Params.ApplySlow(slow);
    }
    
    public void GetBashed(BashInfo bashInfo)
    {
        if (Random.Range(0f, 1f) > bashInfo.chance) return;
        
        for (int i = 0; i < ActiveAbilitiesCD.Count; i++)
        {
            if (Inventory.EquippedActiveAbilitySlots[i].IsEmpty) continue;
            ActiveAbilitiesCD[i] = Inventory.GetEquippedActiveAbility(i).cooldown;
        }
    }
    
    public virtual bool GetStunned(StunInfo stunInfo, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > stunInfo.chance) return false;
        
        isStunned = true;
       
        if (transform.TryGetComponent(out Stun stunComponent))
        {
            stunComponent.AddDuration(stunInfo.Duration);
        }
        else
        {
            var newStun = gameObject.AddComponent<Stun>();
            newStun.Init(stunInfo, this, unitVisual.StunBar, effectIcon);
            ReceiveStatusEffect(newStun);
        }

        return true;
    }

    public virtual void GetUnStunned()
    {
        isStunned = false;
    }
    
    public virtual bool GetFrozen(FreezeInfo freezeInfo, Sprite effectIcon)
    {
        if (TryGetComponent(out Burn burn))
            Destroy(burn);
        
        if (Random.Range(0f, 1f) > freezeInfo.chance) return false;
        
        isFrozen = true;
       
        if (transform.TryGetComponent(out Freeze stunComponent))
        {
            stunComponent.AddDuration(freezeInfo.Duration);
        }
        else
        {
            var newStun = gameObject.AddComponent<Freeze>();
            newStun.Init(freezeInfo, this, unitVisual.StunBar, effectIcon);
            ReceiveStatusEffect(newStun);
        }

        return true;
    }
    
    public virtual void GetUnFrozen()
    {
        isFrozen = false;
    }
    
    public void GetSilence(SilenceInfo silenceInfo, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > silenceInfo.chance) return;

        if (transform.TryGetComponent(out Silence silenceComponent))
        {
            if (silenceComponent.Timer > silenceInfo.Duration) return;
            
            EndSilence();
            Destroy(silenceComponent);
        }
        
        StartSilence();
        var newSilence = gameObject.AddComponent<Silence>();
        newSilence.Init(silenceInfo.Duration, this, effectIcon);
        ReceiveStatusEffect(newSilence);
    }
    
    private void StartSilence()
    {
        IsSilenced = true;
        OnIsSilenceChanged?.Invoke();
    }
    
    public void EndSilence()
    {
        IsSilenced = false;
        OnIsSilenceChanged?.Invoke();
    }
    
    public virtual bool GetPushed(PushInfo pushInfo, Vector2 pushDir, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > pushInfo.chance) return false;

        StartPush();
        if (transform.TryGetComponent(out Push pushComponent))
        {
            EndPush();
            Destroy(pushComponent);
        }
        
        var newPush = gameObject.AddComponent<Push>();
        newPush.Init(pushInfo.PushDuration, this, effectIcon);
        ReceiveStatusEffect(newPush);
        
        rb.velocity = pushDir;
        return true;
    }

    public void StartPush(bool isTrigger = false)
    {
        IsPushing = true;
        coll.isTrigger = isTrigger;
    }
    
    public virtual void EndPush()
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
    
    private int CalculateTakenDamage(DamageInfo damageInfo, float armorPierce)
    {
        int totalDamage = 0;
        
        for (int i = 0; i < damageInfo.GetDamages().Count; i++)
        {
            totalDamage += (int) Mathf.Floor(damageInfo.GetDamages()[i].GetValue() * Params.GetDamageResistance(damageInfo.GetDamages()[i].DamageType));
        }
        
        return (int)Mathf.Floor(totalDamage * (totalDamage / (totalDamage + GetTotalArmor() * (1 - armorPierce))));
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
        OnUnitDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public void Move(Vector3 dir)
    {
        if (IsStunned || IsPushing) return;
        
        rb.MovePosition(rb.position + (Vector2)dir * (MoveSpeed / Params.SlowAmount) * Time.fixedDeltaTime);
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
        if (attackCDTimer > 0 || actionCDTimer > 0 || IsStunned) return;
        
        var newBaseAttack = Instantiate(baseAttackAbility, transform.position, Quaternion.identity);
        newBaseAttack.Execute(this);

        attackCDTimer = 1 / attackSpeed;
        SetActionCD(newBaseAttack.CastTime);

        OnBaseAttack?.Invoke(attackDirAngle, GetWeapon().AttackRadius, GetWeapon().AttackDistance);
    }

    public void Attack(IDamageable damageable)
    {
        throw new NotImplementedException();
    }

    private bool TryToEvade()
    {
        return Random.Range(0f, 1f) < Params.GetEvasionChance();
    }

    public void ExecuteActiveAbility(int index)
    {
        if (actionCDTimer > 0 || ActiveAbilitiesCD[index] > 0 || IsStunned || IsSilenced || Inventory.EquippedActiveAbilitySlots[index].IsEmpty) return;
        ActiveAbility activeAbility = Inventory.GetEquippedActiveAbility(index);
        if (activeAbility.ManaCost > CurrentMana) return;

        SpendMana(activeAbility.ManaCost);
        var newAbility = Instantiate(activeAbility, transform.position, Quaternion.identity);
        newAbility.Execute(this);
        SetActionCD(newAbility.CastTime);
        ActiveAbilitiesCD[index] = newAbility.cooldown;
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
                ActiveAbilitiesCD[i] = Inventory.GetEquippedActiveAbility(i).cooldown;
            }

            lastActiveAbilitiesIDs[i] = newID;
        }
    }

    public void ExecuteExecutableItem(int index)
    {
        if (IsStunned || Inventory.ExecutableSlots[index].IsEmpty) return;

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

    public List<T> GetAllGearPassives<T>()
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

        return res;
    }

    public MeleeWeapon GetWeapon()
    {
        if (Inventory.Equipment.Weapon.IsEmpty)
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

    public Vector2 GetAttackDirection() => lastMoveDir.normalized;
    public float GetAttackDirAngle() => attackDirAngle;
}