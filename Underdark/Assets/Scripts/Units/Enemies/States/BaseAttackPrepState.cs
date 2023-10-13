using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class BaseAttackPrepState : EnemyStateBase
{
    public BaseAttackPrepState(
        bool needsExitTime,
        Enemy Enemy,
        //Action<State<EnemyState, StateEvent>> onEnter,
        float ExitTime) : base(needsExitTime, Enemy, ExitTime/*, onEnter*/) {}
    
    public override void OnEnter()
    {
        Agent.isStopped = true;
        base.OnEnter();
    }
}
