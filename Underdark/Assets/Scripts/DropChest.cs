using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropChest : MonoBehaviour
{
    [SerializeField] protected PlayerSensor playerSensor;
    [SerializeField] protected Drop drop;
    private bool opened;

    void Start()
    {
        playerSensor.OnPlayerEnter += (transform) =>
        {
            if (!opened) drop.DropItems();
            opened = true;
        };
    }
}
