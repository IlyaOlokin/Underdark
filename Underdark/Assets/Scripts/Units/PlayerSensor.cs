using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerSensor : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    public event Action<Transform> OnTargetEnter;
    public event Action<Transform> OnTargetExit;

    public List<Transform> Targets { get; } = new List<Transform>();
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (other.CompareTag("Wall")) return;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position, Mathf.Infinity, layerMask);
            if (hit.collider == null) return;
            
            if (hit.transform.CompareTag("Wall"))
            {
                Targets.Remove(other.transform);
                OnTargetExit?.Invoke(other.transform);
            }
            else if (!Targets.Contains(hit.transform))
            {
                Targets.Add(other.transform);
                OnTargetEnter?.Invoke(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (other.CompareTag("Wall")) return;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, other.transform.position - transform.position, Mathf.Infinity, layerMask);
            if (hit.collider != null && !hit.transform.CompareTag("Wall"))
            {
                Targets.Remove(other.transform);
                OnTargetExit?.Invoke(other.transform);
            }
        }
    }
}
