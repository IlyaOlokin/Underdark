using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker
{
    [SerializeField] private GameObject visuals;
    private bool facingRight = true;
    private Rigidbody2D rb;
    public UnitStats Stats { get; private set;}
    
    [field:SerializeField] public int MaxHP { get; private set;}
    public int CurrentHP { get; private set;}
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    
    [field:SerializeField] public int MoveSpeed { get; private set;}

    [field: Header("Attack Setup")] 
    [field:SerializeField] public float AttackSpeed { get; private set;}
    public event Action<float, float, float> OnBaseAttack;
    
    [field:SerializeField] public MeleeWeapon Weapon { get; private set;}
    [SerializeField] private LayerMask attackMask;
    [SerializeField] protected PolygonCollider2D baseAttackCollider;

    [Header("Active Abilities")] 
    [SerializeField] protected List<ActiveAblity> activeAbilities;
    
    protected Vector3 lastMoveDir;
    protected float attackDirAngle;
    protected float attackCDTimer;
    protected float actionCDTimer;
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Stats = GetComponent<UnitStats>();
        CurrentHP = MaxHP;
    }

    private void Start()
    {
        OnMaxHealthChanged?.Invoke(MaxHP);
        OnHealthChanged?.Invoke(CurrentHP);
        SetAttackCollider(Weapon.AttackRadius, Weapon.AttackDistance + 1);
    }

    protected virtual void Update()
    {
        attackCDTimer -= Time.deltaTime;
        actionCDTimer -= Time.deltaTime;
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
        visuals.transform.Rotate(0,180,0);
    }
    
    public virtual void TakeDamage(float damage)
    {
        CurrentHP -= (int) damage;
        if (CurrentHP <= 0) Death();
        OnHealthChanged?.Invoke(CurrentHP);
    }
    
    protected virtual void Death()
    {
        Destroy(gameObject);
    }
    
    public virtual void Move(Vector3 dir)
    {
        dir = dir.normalized;
        rb.MovePosition(rb.position + (Vector2) dir * MoveSpeed * Time.fixedDeltaTime);
        if (dir != Vector3.zero)
            lastMoveDir = dir;
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
            unit.GetComponent<IDamageable>().TakeDamage(Stats.Strength + Weapon.Damage.GetValue());
        }

        attackCDTimer = 1 / AttackSpeed;
        SetActionCD(1 / (AttackSpeed * 2));
        
        OnBaseAttack?.Invoke(attackDirAngle, Weapon.AttackRadius, Weapon.AttackDistance + 1);
    }
    
    private void SetAttackCollider(float radius, float distance)
    {
        int pointStep = 10;
        int pointsCount = (int) radius / pointStep + 1;
        List<Vector2> path = new List<Vector2>();
        float currentPointAngle = -radius / 2f;
        for (int i = 0; i < pointsCount; i++)
        {
            var sin = Mathf.Sin(Mathf.Deg2Rad * currentPointAngle) * distance;
            var cos = Mathf.Cos(Mathf.Deg2Rad * currentPointAngle) * distance;
            path.Add(new Vector2(sin, cos));
            currentPointAngle += pointStep;
        }
        path.Add(baseAttackCollider.transform.localPosition);
        baseAttackCollider.SetPath(0, path);
    }
    
    protected void ExecuteTeActiveAbility(int index)
    {
        if (actionCDTimer > 0) return;
        var newAbility = Instantiate(activeAbilities[index], transform.position, Quaternion.identity);
        newAbility.Execute(this);
        SetActionCD(newAbility.CastTime);
    }

    private void SetActionCD(float cd)
    {
        actionCDTimer = cd;
    } 

    public Vector2 GetAttackDirection() => lastMoveDir.normalized;
}
