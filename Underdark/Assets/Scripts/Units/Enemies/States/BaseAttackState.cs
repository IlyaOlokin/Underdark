using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class BaseAttackState : EnemyStateBase
{
    public BaseAttackState(
        bool needsExitTime,
        Enemy Enemy,
        Action<State<EnemyState, StateEvent>> onEnter,
        float ExitTime) : base(needsExitTime, Enemy, ExitTime, onEnter) {}
}
