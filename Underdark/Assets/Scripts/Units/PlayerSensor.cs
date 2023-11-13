using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerSensor : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    public event Action<Transform> OnPlayerEnter;
    public event Action<Vector3> OnPlayerExit;
    private bool needRayCast;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            needRayCast = true;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (needRayCast && other.TryGetComponent(out Player player))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.transform.CompareTag("Player"))
            {
                OnPlayerEnter?.Invoke(player.transform);
                needRayCast = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}
