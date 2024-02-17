using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : ActiveAbility
{
    [SerializeField] private float dashSpeed;
    [SerializeField] private Collider2D stopCollider;
    [SerializeField] private List<Collider2D> pushColliders;
    [SerializeField] private LayerMask stopMask;
    private float resetTimer = 1f;

    [Header("Visual")] 
    [SerializeField] private List<ParticleSystem> particleSystems;
    
    public override void Execute(Unit caster, int level, Vector2 attackDir,
        List<IDamageable> damageablesToIgnore1 = null)
    {
        base.Execute(caster, level, attackDir);
        
        transform.SetParent(caster.transform);
        
        transform.eulerAngles = new Vector3(0, 0, caster.GetAttackDirAngle(caster.GetLastMoveDir()));
        
        var destinationPoint = FindDestinationPoint(caster);
        InitDamage(caster);

        foreach (var pushCollider in pushColliders)
        {
            pushCollider.GetComponent<PushZone>()
                .Init(caster, damageInfo, debuffInfos.GetValue(abilityLevel).DebuffInfos, abilityLevel);
        }
        
        StartCoroutine(PushCaster(destinationPoint));
    }

    private IEnumerator PushCaster(Vector2 destinationPoint)
    {
        caster.StartPush(true);

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(stopMask);
        List<Collider2D> hits= new List<Collider2D>();
        
        yield return null;
        
        while (Vector2.Distance(caster.transform.position, destinationPoint) > 0.1f)
        {
            var hitCount = Physics2D.OverlapCollider(stopCollider, contactFilter, hits); 
            if (hitCount != 0) break;

            var newPos = Vector3.MoveTowards(caster.transform.position, destinationPoint, dashSpeed * Time.fixedDeltaTime);
            
            caster.GetMoved(newPos - caster.transform.position);
            resetTimer -= Time.fixedDeltaTime;
            if (resetTimer <= 0)
                break;
            yield return null;
        }

        DisableColliders();

        caster.EndPush();
        transform.SetParent(null);
        foreach (var particleSystem in particleSystems)
            particleSystem.Stop();
    }

    private void DisableColliders()
    {
        foreach (var pushCollider in pushColliders)
            pushCollider.enabled = false;
        stopCollider.enabled = false;
    }

    private Vector2 FindDestinationPoint(Unit caster)
    {
        Vector2 destinationPoint = transform.position;

        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(stopMask);
        var hits = new List<RaycastHit2D>();

       Physics2D.Raycast(transform.position, caster.GetLastMoveDir(), contactFilter, hits,
            AttackDistance.GetValue(abilityLevel));

        bool wallHit = false;
        foreach (var hit in hits)
        {
            wallHit = true;
            destinationPoint = hit.centroid;
            break;
        }

        if (!wallHit) destinationPoint += caster.GetLastMoveDir() * AttackDistance.GetValue(abilityLevel);
        return destinationPoint;
    }
}
