using UnityEngine;
using UnityHFSM;

public class MeleeAbilityEnemy : Enemy
{
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
        // base attack
        EnemyFSM.AddState(EnemyState.AttackPrep, new BaseAttackPrepState(true, this, UnitVisual.StartAlert, meleeAttackPreparation));
        EnemyFSM.AddState(EnemyState.ActiveAbilityExecute, new ActiveAbilityExecuteState(true, this, ExecuteActiveAbility, meleeAttackDuration));
        EnemyFSM.AddState(EnemyState.BaseAttack, new BaseAttackState(true, this, Attack, meleeAttackDuration));
    }
    
    private void AddTransitions()
    {
        EnemyFSM.AddTriggerTransition(StateEvent.StartChase, new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase));
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase,
            (transition) => isPlayerInChasingRange
                            && Vector3.Distance(moveTarget.transform.position, transform.position) > agent.stoppingDistance
                            && !IsDisabled)
        );
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle,
            (transition) => !isPlayerInChasingRange
                            && Vector3.Distance(moveTarget.transform.position, transform.position) <= agent.stoppingDistance)
        );
        

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.AttackPrep, ShouldAttack,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.AttackPrep, ShouldAttack,
            forceInstantly: true));
        
        
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.ActiveAbilityExecute, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.ActiveAbilityExecute, EnemyState.Idle, IsWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsWithinIdleRange));
        
        
        // stun
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.ActiveAbilityExecute, ShouldUseActiveAbility));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.BaseAttack, ShouldMelee));

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.AttackPrep, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
    }
}
