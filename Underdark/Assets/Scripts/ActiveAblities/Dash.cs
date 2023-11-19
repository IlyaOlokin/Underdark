using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : ActiveAbility
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private Collider2D stopCollider;
    private float resetTimer = 1f;
    
    public override void Execute(Unit caster)
    {
        base.Execute(caster);
        
        transform.SetParent(caster.transform);
        var angle = Vector2.Angle(Vector2.right, caster.GetAttackDirection());
        if (caster.GetAttackDirection().y < 0) angle *= -1;
        transform.eulerAngles = new Vector3(0, 0, angle);
        
        var destinationPoint = FindDestinationPoint(caster);
        
        StartCoroutine(PushCaster(destinationPoint));
        
        
    }

    private IEnumerator PushCaster(Vector2 destinationPoint)
    {
        caster.StartPush();

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hits= new List<Collider2D>();
        
        while (Vector2.Distance(caster.transform.position, destinationPoint) > 0.1f)
        {
            var hitCount = Physics2D.OverlapCollider(stopCollider, contactFilter, hits); 
            if (hitCount != 0) break;
            
            caster.GetMoved((destinationPoint - (Vector2) caster.transform.position).normalized * (dashSpeed * Time.fixedDeltaTime));
            resetTimer -= Time.fixedDeltaTime;
            if (resetTimer <= 0)
                break;
            yield return null;
        }

        caster.EndPush();
    }

    private Vector2 FindDestinationPoint(Unit caster)
    {
        Vector2 destinationPoint = transform.position;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        var hits = new List<RaycastHit2D>();

       Physics2D.Raycast(transform.position, caster.GetAttackDirection(), contactFilter, hits,
            AttackDistance);

        bool wallHit = false;
        foreach (var hit in hits)
        {
            wallHit = true;
            destinationPoint = hit.centroid;
            break;
        }

        if (!wallHit) destinationPoint += caster.GetAttackDirection() * AttackDistance;
        return destinationPoint;
    }
}
