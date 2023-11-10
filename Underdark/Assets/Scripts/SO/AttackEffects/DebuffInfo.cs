using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebuffInfo : ScriptableObject
{
    [Range(0f, 1f)] public float chance;
    protected Unit caster;

    public abstract void Execute(IAttacker caster, Unit receiver, Unit unitCaster);

    public new abstract string ToString();
}