using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : ActiveAbility
{
    private Rigidbody2D rb;
    
    [SerializeField] private float distanceFromCasterToDestroy;
    [SerializeField] private float projSpeed;
    private Unit tagetToFollow;
    private bool attachedToUnit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);

        rb.velocity = caster.GetAttackDirection() * projSpeed;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, caster.transform.position) > distanceFromCasterToDestroy)
            Destroy(gameObject);
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
                tagetToFollow.OnUnitDeath += () => Destroy(gameObject);
            }
            Destroy(rb);
        }
    }
}
