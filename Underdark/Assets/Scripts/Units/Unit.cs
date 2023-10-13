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
    
    [field:SerializeField] public int MaxHP { get; private set;}
    public int CurrentHP { get; private set;}
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    
    [field:SerializeField] public int MoveSpeed { get; private set;}
    public int Damage { get; private set;}
    [field:SerializeField] public float AttackSpeed { get; private set;}
    public event Action<float, float, float> OnBaseAttack;
    
    [SerializeField] private MeleeWeapon weapon;
    protected Vector3 lastMoveDir;
    [SerializeField] private LayerMask attackMask;
    [SerializeField] protected PolygonCollider2D baseAttackCollider;
    protected float attackDirAngle;
    protected float attackCDTimer;

    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHP = MaxHP;
    }

    private void Start()
    {
        OnMaxHealthChanged?.Invoke(MaxHP);
        OnHealthChanged?.Invoke(CurrentHP);
        SetAttackCollider(weapon.AttackRadius);
    }

    protected virtual void Update()
    {
        attackCDTimer -= Time.deltaTime;
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
    
    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
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
        //TryFlipVisual(dir.x);
    }
    
    
    public virtual void Attack()
    {
        if (attackCDTimer > 0) return;
        
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitUnits = new List<Collider2D>();
        baseAttackCollider.OverlapCollider(contactFilter, hitUnits);
        
        foreach (var unit in hitUnits)
        {
            unit.GetComponent<IDamageable>().TakeDamage(weapon.Damage);
        }

        attackCDTimer = 1 / AttackSpeed;
        
        OnBaseAttack?.Invoke(attackDirAngle, weapon.AttackRadius, weapon.AttackDistance);
    }
    
    private void SetAttackCollider(float radius)
    {
        int pointStep = 10;
        int pointsCount = (int) radius / pointStep + 1;
        List<Vector2> path = new List<Vector2>();
        float currentPointAngle = -radius / 2f;
        for (int i = 0; i < pointsCount; i++)
        {
            var sin = Mathf.Sin(Mathf.Deg2Rad * currentPointAngle) * weapon.AttackDistance;
            var cos = Mathf.Cos(Mathf.Deg2Rad * currentPointAngle) * weapon.AttackDistance;
            path.Add(new Vector2(sin, cos));
            currentPointAngle += pointStep;
        }
        path.Add(baseAttackCollider.transform.localPosition);
        baseAttackCollider.SetPath(0, path);
    }
}
