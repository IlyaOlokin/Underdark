using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour, IDamageable, IMover, IAttacker
{
    [SerializeField] private GameObject visuals;
    private bool facingRight = true;
    protected Rigidbody2D rb;
    
    [field:SerializeField] public int MaxHP { get; private set;}
    public int CurrentHP { get; private set;}
    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;

    [field:SerializeField] public int MoveSpeed { get; private set; }
    
    [field:SerializeField] public GameObject AttackPrefab { get; private set; }
    [field:SerializeField] public int Damage { get; private set;}
    
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CurrentHP = MaxHP;
        OnMaxHealthChanged?.Invoke(MaxHP);
        OnHealthChanged?.Invoke(CurrentHP);
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
        throw new System.NotImplementedException();
    }
    
    public virtual void Attack()
    {
        throw new System.NotImplementedException();
    }
}
