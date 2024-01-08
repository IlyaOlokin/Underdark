using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class RangeEnemy : Enemy
{
    [Header("Range Enemy Setup")] 
    [SerializeField] private float distToForceMelee;
    [SerializeField] private float distToKeep;

    protected override void Awake()
    {
        base.Awake();
        
        AddStates();
        AddTransitions();
        EnemyFSM.SetStartState(EnemyState.Idle);
        EnemyFSM.Init();
    }
    
    private void AddStates()
    {
        // idle chase
        EnemyFSM.AddState(EnemyState.Idle, new IdleState(false, this));
        EnemyFSM.AddState(EnemyState.Chase, new ChaseState(true, this, moveTarget, ChaseTarget));
        EnemyFSM.AddState(EnemyState.KeepDistance, new KeepDistanceState(true, this, moveTarget, KeepDistanceToTarget));
        // base attack
        EnemyFSM.AddState(EnemyState.AttackPrep, new BaseAttackPrepState(true, this, unitVisual.StartAlert, meleeAttackPreparation));
        EnemyFSM.AddState(EnemyState.ActiveAbilityExecute, new ActiveAbilityExecuteState(true, this, ExecuteActiveAbility, meleeAttackDuration));
        EnemyFSM.AddState(EnemyState.BaseAttack, new BaseAttackState(true, this, Attack, meleeAttackDuration));
    }
    
    private void KeepDistanceToTarget()
    {
        if (isPlayerInChasingRange)
        {
            moveTarget.transform.position = player.transform.position + (transform.position - player.transform.position).normalized * distToKeep;
        }
    }
    
    private void AddTransitions()
    {
        EnemyFSM.AddTriggerTransition(StateEvent.StartChase, new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase));
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase,
            (transition) => isPlayerInChasingRange
                            && Vector3.Distance(moveTarget.transform.position, transform.position) > agent.stoppingDistance
                            && !IsStunned)
        );
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle,
            (transition) => !isPlayerInChasingRange
                            && Vector3.Distance(moveTarget.transform.position, transform.position) <= agent.stoppingDistance)
        );
        

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.AttackPrep, ShouldMelee,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase, IsCloseEnoughToForceMelee,
            forceInstantly: true));
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.KeepDistance, EnemyState.AttackPrep, ShouldUseActiveAbility,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.KeepDistance, EnemyState.Chase, IsCloseEnoughToForceMelee,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.KeepDistance, ShouldKeepDistance,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase, CanNotAnyUseActiveAbility,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.KeepDistance, EnemyState.Chase, CanNotAnyUseActiveAbility,
            forceInstantly: true));
        
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.ActiveAbilityExecute, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.ActiveAbilityExecute, EnemyState.Idle, IsWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsWithinIdleRange));
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.ActiveAbilityExecute, ShouldUseActiveAbility));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.BaseAttack, ShouldMelee));
        
        // stun
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.KeepDistance, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
    }

    private bool IsCloseEnoughToForceMelee(Transition<EnemyState> transition)
    {
        return DistToTargetPos() < distToForceMelee;
    }
    
    private bool ShouldKeepDistance(Transition<EnemyState> transition)
    {
        return !IsCloseEnoughToForceMelee(transition)
               && (CanUseActiveAbility(transition, 0)
               || CanUseActiveAbility(transition, 1)
               || CanUseActiveAbility(transition, 2)
               || CanUseActiveAbility(transition, 3));
    }
}
