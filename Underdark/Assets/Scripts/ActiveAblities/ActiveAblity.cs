using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ActiveAblity : MonoBehaviour
{
    [SerializeField] protected LayerMask attackMask;
    [SerializeField] protected int attackDistance;
    [SerializeField] private float attackAngle;
    
    public virtual void Execute(Unit caster)
    {
        Instantiate(gameObject, transform.position, Quaternion.identity);
    }
    
    protected Collider2D FindClosestTarget(Unit caster)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(caster.transform.position, attackDistance, contactFilter, hitColliders);

        Collider2D target = null;
        float minDist = float.MaxValue;
        foreach (var collider in hitColliders)
        {
            Vector3 dir = collider.transform.position - caster.transform.position;
            var angle = Vector2.Angle(dir, caster.GetAttackDirection());
            if (angle < attackAngle / 2f && dir.magnitude < minDist)
            {
                minDist = dir.magnitude;
                target = collider;
            }
        }

        return target;
    }
}
