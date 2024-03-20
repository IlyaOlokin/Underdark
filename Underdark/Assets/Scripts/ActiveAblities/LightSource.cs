using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : ActiveAbility
{
    private Rigidbody2D rb;
    
    [SerializeField] private float distanceFromCasterToDestroy;
    [SerializeField] private float projSpeed;

    [Header("Visual")] 
    [SerializeField] private LightSourceVisual lightSourceVisual;
    
    private Unit tagetToFollow;
    private bool attachedToUnit;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        lightSourceVisual.LightUp();
    }
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null,bool mustAggro = true)
    {
        base.Execute(caster, level, attackDir);

        rb.velocity = attackDir * projSpeed;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, caster.transform.position) > distanceFromCasterToDestroy)
        {
            lightSourceVisual.LightDown();
            Destroy(gameObject);
        }
    }

    private void LateUpdate()
    {
        if (attachedToUnit)
            transform.position = tagetToFollow.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (caster.AttackMask == (caster.AttackMask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out Unit unit))
            {
                tagetToFollow = unit;
                attachedToUnit = true;
                tagetToFollow.OnDeath += () => Destroy(gameObject);
            }
            Destroy(rb);
        }
    }
}
