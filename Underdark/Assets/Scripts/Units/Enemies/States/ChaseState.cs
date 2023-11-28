using System;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class ChaseState : EnemyStateBase
{
    private Transform target;
    private Enemy enemy;

    public ChaseState(bool needsExitTime, Enemy Enemy, Transform Target, Action onLogic) : base(needsExitTime, Enemy, onLogic: onLogic) 
    {
        target = Target;
        enemy = Enemy;
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
