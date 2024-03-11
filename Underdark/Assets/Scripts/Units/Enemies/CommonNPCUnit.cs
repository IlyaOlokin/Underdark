using UnityEngine;
using UnityHFSM;

public class CommonNPCUnit : NPCUnit
{
    protected override void Awake()
    {
        base.Awake();
        
        AddStates();
        AddTransitions();
        NPCFSM.SetStartState(NPCState.Idle);
        NPCFSM.Init();
    }
    
    private void AddStates()
    {
        // idle chase
        NPCFSM.AddState(NPCState.Idle, new IdleState(false, this, anim));
        NPCFSM.AddState(NPCState.Chase, new ChaseState(true, this, anim, moveTarget, ChaseTarget));
        // base attack
        NPCFSM.AddState(NPCState.AttackPrep, new BaseAttackPrepState(true, this, anim, UnitVisual.StartAlert, meleeAttackPreparation));
        NPCFSM.AddState(NPCState.ActiveAbilityExecute, new ActiveAbilityExecuteState(true, this, anim, ExecuteActiveAbility, meleeAttackDuration));
        NPCFSM.AddState(NPCState.BaseAttack, new BaseAttackState(true, this, anim, Attack, meleeAttackDuration));
    }
    
    private void AddTransitions()
    {
        NPCFSM.AddTriggerTransition(StateEvent.StartChase, new Transition<NPCState>(NPCState.Idle, NPCState.Chase));
        
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.Idle, NPCState.Chase,
            (transition) => DistToMovePos() > agent.stoppingDistance && !IsDisabled)
        );
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.Chase, NPCState.Idle,
            (transition) => Vector2.Distance(agent.destination, transform.position) <= agent.stoppingDistance)
        );
        
        
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.Chase, NPCState.AttackPrep, ShouldAttack,
            forceInstantly: true));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.Idle, NPCState.AttackPrep, ShouldAttack,
            forceInstantly: true));
        
        
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.ActiveAbilityExecute, NPCState.Chase, IsNotWithinIdleRange));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.ActiveAbilityExecute, NPCState.Idle, IsWithinIdleRange));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.BaseAttack, NPCState.Chase, IsNotWithinIdleRange));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.BaseAttack, NPCState.Idle, IsWithinIdleRange));
        
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.AttackPrep, NPCState.ActiveAbilityExecute, ShouldUseActiveAbility));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.AttackPrep, NPCState.BaseAttack, ShouldMelee));
        
        // stun
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.AttackPrep, NPCState.Idle, IsUnitStunned,
            forceInstantly: true));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.BaseAttack, NPCState.Idle, IsUnitStunned,
            forceInstantly: true));
        NPCFSM.AddTransition(new Transition<NPCState>(NPCState.Chase, NPCState.Idle, IsUnitStunned,
            forceInstantly: true));
    }
}
