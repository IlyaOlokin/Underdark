using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    int Damage { get; }
    void Attack();
}
