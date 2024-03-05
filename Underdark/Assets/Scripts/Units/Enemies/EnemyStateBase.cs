using System;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace LlamAcademy.FSM
{
    public abstract class EnemyStateBase : State<NPCState, StateEvent>
    {
        protected readonly NPCUnit NpcUnit;
        protected readonly NavMeshAgent Agent;
        protected readonly Animator Animator;
        protected bool RequestedExit;
        protected float ExitTime;

        protected readonly Action onEnter;
        protected readonly Action onLogic;
        protected readonly Action onExit;
        protected readonly Func<State<NPCState, StateEvent>, bool> canExit;

        public EnemyStateBase(bool needsExitTime, 
            NPCUnit npcUnit,
            Animator Animator,
            float ExitTime = 0.1f,
            Action onEnter = null,
            Action onLogic = null,
            Action onExit = null,
            Func<State<NPCState, StateEvent>, bool> canExit = null)
        {
            this.NpcUnit = npcUnit;
            this.Animator = Animator;
            this.onEnter = onEnter;
            this.onLogic = onLogic;
            this.onExit = onExit;
            this.canExit = canExit;
            this.ExitTime = ExitTime;
            this.needsExitTime = needsExitTime;
            Agent = npcUnit.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            RequestedExit = false;
            onEnter?.Invoke();
        }

        public override void OnLogic()
        {
            base.OnLogic();
            if (RequestedExit && timer.Elapsed >= ExitTime)
            {
                fsm.StateCanExit();
            }
            onLogic?.Invoke();
        }

        public override void OnExitRequest()
        {
            if (!needsExitTime || canExit != null && canExit(this))
            {
                fsm.StateCanExit();
            }
            else
            {
                RequestedExit = true;
            }
        }
    }

}