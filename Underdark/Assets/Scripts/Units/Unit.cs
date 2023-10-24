using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker, ICaster
{
    private Rigidbody2D rb;
    public UnitStats Stats;
    public Inventory Inventory;

    [field: SerializeField] public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    [field: SerializeField] public int MoveSpeed { get; private set; }

    [field: Header("Attack Setup")] [SerializeField]
    private MeleeWeapon defaultWeapon;

    [field: SerializeField] public float AttackSpeed { get; private set; }
    public event Action<float, float, float> OnBaseAttack;

    [SerializeField] private LayerMask attackMask;
    [SerializeField] protected PolygonCollider2D baseAttackCollider;


    [field: Header("Abilities Setup")]
    [field: SerializeField]
    public List<ActiveAblity> ActiveAbilities { get; private set; }

    [field: SerializeField] public List<float> ActiveAbilitiesCD { get; private set; }

    [Header("Visual")] [SerializeField] private GameObject visuals;
    private bool facingRight = true;
    [SerializeField] protected DamageNumberEffect damageNumberEffect;
    [SerializeField] protected UnitVisual unitVisual;

    protected Vector3 lastMoveDir;
    protected float attackDirAngle;
    protected float attackCDTimer;
    private float actionCDTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ActiveAbilitiesCD = new List<float>(new float[ActiveAbilities.Count]);
        SetHP();
        Inventory = new Inventory(10, this);
        Inventory.OnEquipmentChanged += SetAttackCollider;
    }

    private void SetHP()
    {
        MaxHP += Stats.Strength * 10;
        CurrentHP = MaxHP;
    }

    private void Start()
    {
        OnMaxHealthChanged?.Invoke(MaxHP);
        OnHealthChanged?.Invoke(CurrentHP);
        SetAttackCollider();
    }

    protected virtual void Update()
    {
        UpdateCoolDowns();
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

    public virtual void TakeDamage(float damage)
    {
        var newDamage = CalculateDamage(damage);
        CurrentHP -= newDamage;
        unitVisual.StartWhiteOut();
        if (CurrentHP <= 0) Death();
        var newEffect = Instantiate(damageNumberEffect, transform.position, Quaternion.identity);
        newEffect.WriteDamage(newDamage);
        OnHealthChanged?.Invoke(CurrentHP);
    }

    private int CalculateDamage(float damage)
    {
        return (int) Mathf.Floor(damage * (damage / (damage + GetTotalArmor())));
    }

    private int GetTotalArmor()
    {
        var armor = GetArmorAmount(ItemType.Head) +
                    GetArmorAmount(ItemType.Body) +
                    GetArmorAmount(ItemType.Legs);

        return armor;
    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }

    public virtual void Move(Vector3 dir)
    {
        dir = dir.normalized;
        rb.MovePosition(rb.position + (Vector2)dir * MoveSpeed * Time.fixedDeltaTime);
        if (dir != Vector3.zero)
            lastMoveDir = dir;
        TryFlipVisual(dir.x);
    }

    public virtual void Attack()
    {
        if (attackCDTimer > 0 || actionCDTimer > 0) return;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitUnits = new List<Collider2D>();
        baseAttackCollider.OverlapCollider(contactFilter, hitUnits);

        foreach (var unit in hitUnits)
        {
            unit.GetComponent<IDamageable>().TakeDamage(Stats.Strength + GetWeapon().Damage.GetValue());
        }

        attackCDTimer = 1 / AttackSpeed;
        SetActionCD(1 / (AttackSpeed * 2));

        OnBaseAttack?.Invoke(attackDirAngle, GetWeapon().AttackRadius, GetWeapon().AttackDistance);
    }

    private void SetAttackCollider()
    {
        int pointStep = 10;
        int pointsCount = GetWeapon().AttackRadius / pointStep + 1;
        List<Vector2> path = new List<Vector2>();
        float currentPointAngle = -GetWeapon().AttackRadius / 2f;
        for (int i = 0; i < pointsCount; i++)
        {
            var sin = Mathf.Sin(Mathf.Deg2Rad * currentPointAngle) * GetWeapon().AttackDistance;
            var cos = Mathf.Cos(Mathf.Deg2Rad * currentPointAngle) * GetWeapon().AttackDistance;
            path.Add(new Vector2(sin, cos));
            currentPointAngle += pointStep;
        }

        path.Add(baseAttackCollider.transform.localPosition);
        baseAttackCollider.SetPath(0, path);
    }

    public void ExecuteActiveAbility(int index)
    {
        if (actionCDTimer > 0 || ActiveAbilitiesCD[index] > 0) return;
        var newAbility = Instantiate(ActiveAbilities[index], transform.position, Quaternion.identity);
        newAbility.Execute(this);
        SetActionCD(newAbility.CastTime);
        ActiveAbilitiesCD[index] = newAbility.cooldown;
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