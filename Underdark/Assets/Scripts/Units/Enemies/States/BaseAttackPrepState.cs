using System;
using System.Collections;
using System.Collections.Generic;
using LlamAcademy.FSM;
using UnityEngine;
using UnityHFSM;

public class BaseAttackPrepState : EnemyStateBase
{
    public BaseAttackPrepState(bool needsExitTime,
        NPCUnit npcUnit,
        Animator anim,
        Action onEnter,
        float ExitTime
    ) : base(needsExitTime, npcUnit, anim, ExitTime, onEnter) {}
    
    public override void OnEnter()
    {
        if (Agent.enabled)
            Agent.isStopped = true;
        Animator.SetBool("Move", false);

        base.OnEnter();
    }
}
