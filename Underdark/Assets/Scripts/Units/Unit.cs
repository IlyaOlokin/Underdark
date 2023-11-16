using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker, ICaster
{
    private Rigidbody2D rb;
    public UnitStats Stats;
    public Inventory Inventory;
    public EnergyShield EnergyShield;

    private bool isStunned;
    private bool isFrozen;
    protected bool IsStunned => isStunned || isFrozen;
    protected bool IsPushing { get; private set; }

    [SerializeField] private int baseMaxHP;
    public int MaxHP { get; private set; }

    private int _currentHP;

    public int CurrentHP
    {
        get => _currentHP;
        private set
        {
            if (value > MaxHP) value = MaxHP;
            _currentHP = value;
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
        private set
        {
            if (value > MaxMana) value = MaxMana;
            if (value < 0) value = 0;
            _currentMana = value;
        }
    }

    public event Action<int> OnManaChanged;
    public event Action<int> OnMaxManaChanged;

    [SerializeField] [Range(0f, 1f)] private float regenPercent;

    [field: SerializeField] public int MoveSpeed { get; private set; }

    [field: Header("Attack Setup")] [SerializeField]
    private MeleeWeapon defaultWeapon;

    [SerializeField] private float attackSpeed;
    public event Action<float, float, float> OnBaseAttack;
    public event Action<int> OnExecutableItemUse;

    [NonSerialized] public List<IStatusEffect> Buffs = new();
    public event Action<IStatusEffect> OnStatusEffectReceive;
    public event Action<IStatusEffect> OnStatusEffectLoose;

    [SerializeField] protected LayerMask attackMask;
    [SerializeField] protected PolygonCollider2D baseAttackCollider;
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
    
    // Unit Multipliers
    protected float slowAmount = 1;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
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
        SetAttackCollider();
        SetActiveAbilitiesCDs();
        GetUnStunned();
        EndPush();
        LooseEnergyShield();
        foreach (var debuff in GetComponents<IStatusEffect>())
        {
            Destroy((Object)debuff);
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
        MaxMana = baseMaxMana + Stats.Intelligence * 10;

        if (toFull)
            CurrentMana = MaxMana;
        else
            CurrentMana = (int)Mathf.Floor(MaxMana * currentPart);
        
        OnMaxManaChanged?.Invoke(MaxMana);
        OnManaChanged?.Invoke(CurrentMana);
    }

    protected void SetHP(bool toFull = false)
    {
        float currentPart = CurrentHP / (float) MaxHP;
        MaxHP = baseMaxHP + Stats.Strength * 10;
        
        if (toFull)
            CurrentHP = MaxHP;
        else
            CurrentHP = (int)Mathf.Floor(MaxHP * currentPart);

        OnMaxHealthChanged?.Invoke(MaxHP);
        OnHealthChanged?.Invoke(CurrentHP);
    }

    private void UpdateCoolDowns()
    {
        attackCDTimer -= Time.deltaTime;
        actionCDTimer -= Time.deltaTime;

        for (var index = 0; index < ActiveAbilitiesCD.Count; index++)
        {
            ActiveAbilitiesCD[index] -= Time.deltaTime / slowAmount;
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

    public virtual bool TakeDamage(Unit sender, IAttacker attacker, float damage, bool evadable = true, float armorPierce = 0f)
    {
        var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);

        if (evadable && TryToEvade(sender, this))
        {
            newEffect.WriteMessage("Evaded!");
            return false;
        }

        var newDamage = CalculateTakenDamage(damage, armorPierce);

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
        if (CurrentHP <= 0) Death(sender);
        newEffect.WriteDamage(newDamage);
        OnHealthChanged?.Invoke(CurrentHP);
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
        OnHealthChanged?.Invoke(CurrentHP);
    }
    
    public void RestoreMP(int mp)
    {
        CurrentMana += mp;
        OnManaChanged?.Invoke(CurrentMana);
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

    public void GetPoisoned(PoisonInfo poisonInfo, Unit caster, GameObject visual, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > poisonInfo.chance) return;
        
        var newPoison = gameObject.AddComponent<Poison>();
        newPoison.Init(poisonInfo, this, caster, visual, effectIcon);
        ReceiveStatusEffect(newPoison);
    }
    public void GetBleed(BleedInfo bleedInfo, Unit caster, GameObject visual, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > bleedInfo.chance) return;
        
        var newBleed = gameObject.AddComponent<Bleed>();
        newBleed.Init(bleedInfo, this, caster, visual, effectIcon);
        ReceiveStatusEffect(newBleed);
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
        slowAmount = slow;
    }
    public void GetBashed(BashInfo bashInfo)
    {
        if (Random.Range(0f, 1f) > bashInfo.chance) return;
        
        for (int i = 0; i < ActiveAbilitiesCD.Count; i++)
        {
            if (Inventory.EquippedActiveAbilitySlots[i].IsEmpty) continue;
            ActiveAbilitiesCD[i] = Inventory.GetActiveAbility(i).cooldown;
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
    
    public virtual bool GetPushed(PushInfo pushInfo, Vector2 pushDir, Sprite effectIcon)
    {
        if (Random.Range(0f, 1f) > pushInfo.chance) return false;
        
        IsPushing = true;
        if (transform.TryGetComponent(out Push pushComponent))
            Destroy(pushComponent);
        
        var newPush = gameObject.AddComponent<Push>();
        newPush.Init(pushInfo.PushDuration, this, effectIcon);
        ReceiveStatusEffect(newPush);
        
        rb.AddForce(pushDir, ForceMode2D.Impulse);
        return true;
    }
    
    public virtual void EndPush()
    {
        IsPushing = false;
        rb.totalForce = Vector2.zero;
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
    
    private int CalculateTakenDamage(float damage, float armorPierce)
    {
        return (int)Mathf.Floor(damage * (damage / (damage + GetTotalArmor() * (1 - armorPierce))));
    }

    public int GetTotalArmor()
    {
        var armor = GetArmorAmount(ItemType.Head) +
                    GetArmorAmount(ItemType.Body) +
                    GetArmorAmount(ItemType.Legs) + 
                    GetArmorAmount(ItemType.Shield);

        return armor;
    }

    protected virtual void Death(Unit killer)
    {
        OnUnitDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public void Move(Vector3 dir)
    {
        if (IsStunned || IsPushing) return;
        
        rb.MovePosition(rb.position + (Vector2)dir * (MoveSpeed / slowAmount) * Time.fixedDeltaTime);
        if (dir != Vector3.zero)
            lastMoveDir = dir;
        TryFlipVisual(dir.x);
    }
    
    public virtual void Attack()
    {
        if (attackCDTimer > 0 || actionCDTimer > 0 || IsStunned) return;
        
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitUnits = new List<Collider2D>();
        baseAttackCollider.OverlapCollider(contactFilter, hitUnits);

        foreach (var collider in hitUnits)
        {
            if (!HitCheck(collider.transform, contactFilter)) continue;
            
            if (collider.TryGetComponent(out IDamageable unit))
            {
                if (unit.TakeDamage(this, this, GetTotalDamage().GetValue(),
                        armorPierce: GetWeapon().ArmorPierce))
                {
                    foreach (var debuffInfo in GetWeapon().DebuffInfos)
                    {
                        debuffInfo.Execute(this, collider.GetComponent<Unit>(), this);
                    }
                }
            }
        }

        attackCDTimer = 1 / attackSpeed;
        SetActionCD(1 / (attackSpeed * 2));

        OnBaseAttack?.Invoke(attackDirAngle, GetWeapon().AttackRadius, GetWeapon().AttackDistance);
    }
    
    private bool HitCheck(Transform target, ContactFilter2D contactFilter)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        Physics2D.Raycast(transform.position,
            target.position - transform.position,
            contactFilter,
            hits);
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("Wall")) return false;
            if (hit.transform == target) return true;
        }
        
        return true;
    }

    public Damage GetTotalDamage()
    {
        return new Damage(GetWeapon().Damage, Stats.GetTotalStatValue(BaseStat.Strength));
    }

    public void Attack(IDamageable damageable)
    {
        throw new NotImplementedException();
    }

    private bool TryToEvade(Unit attacker, Unit receiver)
    {
        var chance = attacker.Stats.Dexterity / (float)(attacker.Stats.Dexterity + receiver.Stats.Dexterity);
        return Random.Range(0f, 1f) > chance;
    }

    protected void SetAttackCollider()
    {
        int pointStep = 10;
        int pointsCount = GetWeapon().AttackRadius / pointStep + 1;
        List<Vector2> path = new List<Vector2>();
        float currentPointAngle = -GetWeapon().AttackRadius / 2f;
        for (int i = 0; i < pointsCount; i++)
        {
            var sin = Mathf.Sin(Mathf.Deg2Rad * currentPointAngle) * (GetWeapon().AttackDistance + 1);
            var cos = Mathf.Cos(Mathf.Deg2Rad * currentPointAngle) * (GetWeapon().AttackDistance + 1);
            path.Add(new Vector2(sin, cos));
            currentPointAngle += pointStep;
        }

        path.Add(baseAttackCollider.transform.localPosition);
        baseAttackCollider.SetPath(0, path);
    }

    public void ExecuteActiveAbility(int index)
    {
        if (actionCDTimer > 0 || ActiveAbilitiesCD[index] > 0 || IsStunned || Inventory.EquippedActiveAbilitySlots[index].IsEmpty) return;
        ActiveAbility activeAbility = Inventory.GetActiveAbility(index);
        if (activeAbility.ManaCost > CurrentMana) return;

        SpendMana(activeAbility.ManaCost);
        var newAbility = Instantiate(activeAbility, transform.position, Quaternion.identity);
        newAbility.Execute(this);
        SetActionCD(newAbility.CastTime);
        ActiveAbilitiesCD[index] = newAbility.cooldown;
    }

    protected void SetActiveAbilitiesCDs()
    {
        for (int i = 0; i < Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            string newID = Inventory.EquippedActiveAbilitySlots[i].IsEmpty ? "" : Inventory.EquippedActiveAbilitySlots[i].ItemID;
            
            if (newID == "" && lastActiveAbilitiesIDs[i] != "")
            {
                ActiveAbilitiesCD[i] = 0;
                lastActiveAbilitiesIDs[i] = newID;
                continue;
            }
            if (newID != lastActiveAbilitiesIDs[i])
            {
                ActiveAbilitiesCD[i] = Inventory.GetActiveAbility(i).cooldown;
            }

            lastActiveAbilitiesIDs[i] = newID;
        }
    }

    public void ExecuteExecutableItem(int index)
    {
        if (IsStunned || Inventory.ExecutableSlots[index].IsEmpty) return;
        
        Inventory.GetExecutableItem(index).Execute(this);
        Inventory.Remove(Inventory.ExecutableSlots[index]);
        OnExecutableItemUse?.Invoke(index);
    }

    public void SpendMana(int manaCost)
    {
        CurrentMana -= manaCost;
        OnManaChanged?.Invoke(CurrentMana);
    }

    private void SetActionCD(float cd)
    {
        actionCDTimer = cd;
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
}