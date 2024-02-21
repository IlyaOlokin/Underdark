using System;
using UnityEngine;

public abstract class PassiveSO : ScriptableObject
{
    [field:SerializeField] public Sprite Icon { get; private set; }
}