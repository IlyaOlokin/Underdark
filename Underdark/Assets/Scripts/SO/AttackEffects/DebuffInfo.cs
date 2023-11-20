using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebuffInfo : ScriptableObject
{
    [SerializeField] protected Sprite effectIcon;
    [Range(0f, 1f)] public float chance;
    protected Unit caster;

    public abstract void Execute(IAttacker attacker, Unit receiver, Unit unitCaster);

    public new abstract string ToString();
}
