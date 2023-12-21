using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class ActiveAbilityExecuteState : EnemyStateBase
{
    public ActiveAbilityExecuteState(bool needsExitTime,
        Enemy Enemy,
        Action onEnter,
        float ExitTime) 
        : base(needsExitTime, Enemy, ExitTime, onEnter) {}
    
    public override void OnEnter()
    {
        if (Agent.enabled)
            Agent.isStopped = true;
        base.OnEnter();
    }
}
