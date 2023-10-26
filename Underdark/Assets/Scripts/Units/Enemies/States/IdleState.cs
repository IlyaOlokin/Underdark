using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;

public class IdleState : EnemyStateBase
{
    public IdleState(bool needsExitTime, Enemy Enemy) : base(needsExitTime, Enemy) { }
    
    public override void OnEnter()
    {
        base.OnEnter();
        if (Agent.enabled) Agent.isStopped = true;
    }
}
