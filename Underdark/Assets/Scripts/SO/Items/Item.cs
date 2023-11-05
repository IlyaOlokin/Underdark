using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : ScriptableObject
{
    public string ID;
    public ItemType ItemType;
    public Sprite Sprite;
    public int StackCapacity;
    public Requirements Requirements;

    public new string ToString()
    {
        return name;
    }
}
