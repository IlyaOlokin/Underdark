using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;

public class IdleState : EnemyStateBase
{
    private float AnimationLoopCount = 0;

    public IdleState(bool needsExitTime, Enemy Enemy) : base(needsExitTime, Enemy) { }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Agent.isStopped = true;
    }
}
