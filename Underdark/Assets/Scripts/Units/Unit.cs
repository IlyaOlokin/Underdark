using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker, ICaster, IPoisonable, IStunable, IBleedable, IPushable
{
    private Rigidbody2D rb;
    public UnitStats Stats;
    public Inventory Inventory;
    private List<Debuff> Debuffs;

    public bool IsStunned { get; private set; }
    public bool IsPushing { get; private set; }

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

    [SerializeField] private LayerMask attackMask;
    [SerializeField] protected PolygonCollider2D baseAttackCollider;
    public Transform Transform => transform;

    [field: Header("Abilities Setup")]
    private string[] lastActiveAbilitiesIDs;

    [field: SerializeField] public List<float> ActiveAbilitiesCD { get; private set; }

    [Header("Visual")] 
    [SerializeField] private GameObject visuals;
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
        
        Inventory = new Inventory(10, 14, this);
        Inventory.OnEquipmentChanged += SetAttackCollider;
        Inventory.OnActiveAbilitiesChanged += SetActiveAbilitiesCDs;
        Stats.OnStatsChanged += SetHP;
        Stats.OnStatsChanged += SetMana;
        Stats.OnLevelUp += OnLevelUp;
        
        ActiveAbilitiesCD = new List<float>(new float[Inventory.EquippedActiveAbilitySlots.Count]);
        lastActiveAbilitiesIDs = new string[Inventory.EquippedActiveAbilitySlots.Count];
    }

    private void SetMana(bool toFull = false)
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

    private void SetHP(bool toFull = false)
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

    private void Start()
    {
        SetHP(true);
        SetMana(true);
        SetAttackCollider();
        SetActiveAbilitiesCDs();
    }

    protected virtual void Update()
    {
        UpdateCoolDowns();
        Regeneration();
    }

    private void UpdateCoolDowns()
    {
        attackCDTimer -= Time.deltaTime;
        actionCDTimer -= Time.deltaTime;

        for (var index = 0; index < ActiveAbilitiesCD.Count; index++)
        {
            ActiveAbilitiesCD[index] -= Time.deltaTime;
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
                RestoreMana((int)Mathf.Floor(manaRegenBuffer));
                manaRegenBuffer %= 1;
            }
        }
        else
        {
            manaRegenBuffer = 0;
        }
    }
    
    private void OnLevelUp()
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
        visuals.transform.Rotate(0, 180, 0);
    }

    public virtual bool TakeDamage(Unit sender, float damage, bool evadable = true, float armorPierce = 0f)
    {
        var newEffect = Instantiate(unitNotificationEffect, transform.position, Quaternion.identity);

        if (evadable && TryToEvade(sender, this))
        {
            newEffect.WriteMessage("Evaded!");
            return false;
        }

        var newDamage = CalculateTakenDamage(damage, armorPierce);
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

    public void GetPoisoned(PoisonInfo poisonInfo, Unit caster, GameObject visual)
    {
        if (Random.Range(0f, 1f) < poisonInfo.chance)
        {
            var newPoison = gameObject.AddComponent<Poison>();
            newPoison.Init(poisonInfo, this, caster, visual);
        }
    }
    public void GetBleed(BleedInfo bleedInfo, Unit caster)
    {
        if (Random.Range(0f, 1f) < bleedInfo.chance)
        {
            var newBleed = gameObject.AddComponent<Bleed>();
            newBleed.Init(bleedInfo, this, caster);
        }
    }
    
    public virtual bool GetStunned(StunInfo stunInfo)
    {
        if (Random.Range(0f, 1f) > stunInfo.chance) return false;
        
        IsStunned = true;
       
        if (transform.TryGetComponent(out Stun stunComponent))
        {
            stunComponent.AddDuration(stunInfo.Duration);
        }
        else
        {
            var newStun = gameObject.AddComponent<Stun>();
            newStun.Init(stunInfo, this, unitVisual.StunBar);
        }

        return true;
    }

    public virtual void GetUnStunned()
    {
        IsStunned = false;
    }
    
    public virtual bool GetPushed(PushInfo pushInfo, Vector2 pushDir)
    {
        if (Random.Range(0f, 1f) > pushInfo.chance) return false;
        
        IsPushing = true;
        if (transform.TryGetComponent(out Push pushComponent))
            Destroy(pushComponent);
        
        var newPush = gameObject.AddComponent<Push>();
        newPush.Init(1f, this);
        
        rb.AddForce(pushDir, ForceMode2D.Impulse);
        return true;
    }

    public virtual void EndPush()
    {
        IsPushing = false;
    }

    private int CalculateTakenDamage(float damage, float armorPierce)
    {
        return (int)Mathf.Floor(damage * (damage / (damage + GetTotalArmor() * (1 - armorPierce))));
    }

    private int GetTotalArmor()
    {
        var armor = GetArmorAmount(ItemType.Head) +
                    GetArmorAmount(ItemType.Body) +
                    GetArmorAmount(ItemType.Legs);

        return armor;
    }

    protected virtual void Death(Unit killer)
    {
        Destroy(gameObject);
    }

    public virtual void Move(Vector3 dir)
    {
        if (IsStunned || IsPushing) return;

        dir = dir.normalized;
        rb.MovePosition(rb.position + (Vector2)dir * MoveSpeed * Time.fixedDeltaTime);
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

        foreach (var unit in hitUnits)
        {
            if (unit.GetComponent<IDamageable>().TakeDamage(this, Stats.Strength + GetWeapon().Damage.GetValue(),
                    armorPierce: GetWeapon().ArmorPierce))
            {
                foreach (var debuffInfo in GetWeapon().DebuffInfos)
                {
                    debuffInfo.Execute(this, unit.GetComponent<Unit>(), this);
                }
            }
        }

        attackCDTimer = 1 / attackSpeed;
        SetActionCD(1 / (attackSpeed * 2));

        OnBaseAttack?.Invoke(attackDirAngle, GetWeapon().AttackRadius, GetWeapon().AttackDistance);
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

    private void SetAttackCollider()
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

    private void SetActiveAbilitiesCDs()
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

    private void RestoreMana(int mana)
    {
        CurrentMana += mana;
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

    public Armor GetArmor(ItemType itemType) => Inventory.Equipment.GetArmor(itemType);

    private int GetArmorAmount(ItemType itemType)
    {
        var armor = GetArmor(itemType);
        if (armor == null)
            return 0;

        return armor.ArmorAmount;
    }

    public Vector2 GetAttackDirection() => lastMoveDir.normalized;
}