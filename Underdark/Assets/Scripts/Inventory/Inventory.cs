using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    [SerializeField] private Item[] items;
    [SerializeField] private Equipment equipment;
}
