using System;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace LlamAcademy.FSM
{
    public abstract class EnemyStateBase : State<EnemyState, StateEvent>
    {
        protected readonly Enemy Enemy;
        protected readonly NavMeshAgent Agent;
        //protected readonly Animator Animator;
        protected bool RequestedExit;
        protected float ExitTime;

        protected readonly Action onEnter;
        protected readonly Action onLogic;
        protected readonly Action onExit;
        protected readonly Func<State<EnemyState, StateEvent>, bool> canExit;

        public EnemyStateBase(bool needsExitTime, 
            Enemy Enemy, 
            float ExitTime = 0.1f,
            Action onEnter = null,
            Action onLogic = null,
            Action onExit = null,
            Func<State<EnemyState, StateEvent>, bool> canExit = null)
        {
            this.Enemy = Enemy;
            this.onEnter = onEnter;
            this.onLogic = onLogic;
            this.onExit = onExit;
            this.canExit = canExit;
            this.ExitTime = ExitTime;
            this.needsExitTime = needsExitTime;
            Agent = Enemy.GetComponent<NavMeshAgent>();
            //Animator = Enemy.GetComponent<Animator>();
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