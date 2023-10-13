using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public interface IAttacker
{
    int Damage { get; }
    float AttackSpeed { get; }

    void Attack(State<EnemyState, StateEvent> State = null);
}
