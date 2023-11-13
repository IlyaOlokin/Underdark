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
    
    private bool foundPlayer;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, layerMask);
            if (hit.collider == null) return;
            
            if (hit.transform.CompareTag("Player"))
            {
                OnPlayerEnter?.Invoke(player.transform);
                foundPlayer = true;
            }
            else if (foundPlayer && hit.transform.CompareTag("Wall"))
            {
                OnPlayerExit?.Invoke(player.transform.position);
                foundPlayer = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.transform.CompareTag("Player"))
                OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}
