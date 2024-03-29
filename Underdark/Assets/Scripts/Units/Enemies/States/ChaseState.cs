using System;
using LlamAcademy.FSM;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : EnemyStateBase
{
    private Transform target;
    private NPCUnit _npcUnit;

    public ChaseState(bool needsExitTime, NPCUnit npcUnit, Animator anim, Transform Target, Action onLogic = null) : base(needsExitTime, npcUnit, anim, onLogic: onLogic) 
    {
        target = Target;
        _npcUnit = npcUnit;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Agent.enabled = true;
        if (Agent.isOnNavMesh) Agent.isStopped = false;
        Animator.SetBool("Move", true);
    }
    
    public override void OnLogic()
    {
        base.OnLogic();
        if (!RequestedExit)
        {
            if (!Agent.enabled) return;
            if (NavMesh.SamplePosition(target.position, out var navHit, 100f, NavMesh.AllAreas))
                Agent.SetDestination(navHit.position);
        }
        else if (!Agent.enabled || Agent.remainingDistance <= Agent.stoppingDistance)
        {
            fsm.StateCanExit();
        }
    }
}
