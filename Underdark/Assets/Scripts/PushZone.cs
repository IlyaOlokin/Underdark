using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushZone : MonoBehaviour, IAttacker
{
    public Transform Transform => transform;
    
    [SerializeField] private LayerMask mask;
    [SerializeField] private PushInfo pushInfo;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mask == (mask | (1 << other.gameObject.layer)))
        {
            if (other.TryGetComponent(out Unit unit))
            {
                pushInfo.Execute(this, unit, null);
            }
        }
    }

    public void Attack()
    {
        throw new NotImplementedException();
    }

    public void Attack(IDamageable unit)
    {
        throw new NotImplementedException();
    }
}
