using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class KeepDistanceState : EnemyStateBase
{
    private Transform target;
    private Enemy enemy;
    
    public KeepDistanceState(bool needsExitTime, Enemy Enemy, Transform Target, Action onLogic) : base(needsExitTime, Enemy, onLogic: onLogic)
    {
        target = Target;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Agent.enabled = true;
        Agent.isStopped = false;
    }
    
    public override void OnLogic()
    {
        base.OnLogic();
        if (!RequestedExit)
        {
            if (Agent.enabled)
                Agent.SetDestination(target.position);
        }
        else if (!Agent.enabled || Agent.remainingDistance <= Agent.stoppingDistance)
        {
            fsm.StateCanExit();
        }
    }
}
