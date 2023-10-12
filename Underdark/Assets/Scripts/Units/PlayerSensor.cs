using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerSensor : MonoBehaviour
{
    public event Action<Transform> OnPlayerEnter;
    public event Action<Vector3> OnPlayerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            OnPlayerEnter?.Invoke(player.transform);
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
