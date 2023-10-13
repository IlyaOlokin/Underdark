using System;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class ChaseState : EnemyStateBase
{
    private Transform Target;

    public ChaseState(bool needsExitTime, Enemy Enemy, Transform Target) : base(needsExitTime, Enemy) 
    {
        this.Target = Target;
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
            Agent.SetDestination(Target.position);
        }
        else if (Agent.remainingDistance <= Agent.stoppingDistance)
        {
            fsm.StateCanExit();
        }
    }
}
