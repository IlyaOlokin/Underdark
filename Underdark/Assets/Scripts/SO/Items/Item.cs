using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Item : ScriptableObject
{
    public string ID;
    public string Name;
    public int Cost;
    public ItemType ItemType;
    public Sprite Sprite;
    public int StackCapacity;
    public Requirements Requirements;
    
    public virtual string[] ToString(Unit owner)
    {
        throw new NotImplementedException();
    }
    
    public  virtual string[] ToStringAdditional(Unit owner)
    {
        return new string[]{};
    }

}
