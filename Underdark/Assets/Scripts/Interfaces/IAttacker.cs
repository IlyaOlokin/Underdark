using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public interface IAttacker
{
    float AttackSpeed { get; }

    void Attack();
}
