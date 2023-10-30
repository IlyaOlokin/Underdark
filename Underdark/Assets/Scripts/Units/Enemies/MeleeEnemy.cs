using Unity.VisualScripting;
using UnityEngine;
using UnityHFSM;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float meleeAttackDuration;
    [SerializeField] private float meleeAttackPreparation;
    
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
        EnemyFSM.AddState(EnemyState.Idle, new IdleState(false, this));
        EnemyFSM.AddState(EnemyState.Chase, new ChaseState(true, this, moveTarget));
        EnemyFSM.AddState(EnemyState.BaseAttackPrep, new BaseAttackPrepState(true, this, unitVisual.StartAlert, meleeAttackPreparation));
        EnemyFSM.AddState(EnemyState.BaseAttack, new BaseAttackState(true, this, Attack, meleeAttackDuration));
    }
    
    private void AddTransitions()
    {
        EnemyFSM.AddTriggerTransition(StateEvent.DetectPlayer, new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase));
        //EnemyFSM.AddTriggerTransition(StateEvent.LostPlayer, new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase,
            (transition) => isPlayerInChasingRange
                            && Vector3.Distance(player.transform.position, transform.position) > agent.stoppingDistance
                            && !IsStunned)
        );
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle,
            (transition) => !isPlayerInChasingRange
                            && Vector3.Distance(player.transform.position, transform.position) <= agent.stoppingDistance)
        );

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.BaseAttackPrep, ShouldMelee ,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.BaseAttackPrep, ShouldMelee,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttackPrep, EnemyState.BaseAttack));

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttackPrep, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.BaseAttack, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle, IsUnitStunned,
            forceInstantly: true));
    }
}
