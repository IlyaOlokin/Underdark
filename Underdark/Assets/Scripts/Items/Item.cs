using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : ScriptableObject
{
    public string ID;
    public ItemType ItemType;
    public Sprite sprite;
    public int StackCapacity;

}
