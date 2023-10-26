using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityHFSM;
using Zenject;

public class Enemy : Unit
{
    protected Player player;
    [SerializeField] protected NavMeshAgent agent;
    protected StateMachine<EnemyState, StateEvent> EnemyFSM;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    
    protected bool isPlayerInMeleeRange;
    protected bool isPlayerInChasingRange;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        EnemyFSM = new();
        followPlayerSensor.OnPlayerEnter += FollowPlayerSensor_OnPlayerEnter;
        followPlayerSensor.OnPlayerExit += FollowPlayerSensor_OnPlayerExit;
    }

    protected override void Update()
    {
        base.Update();
        EnemyFSM.OnLogic();
        RotateAttackDir();
        TryFlipVisual(agent.desiredVelocity.x);
        Debug.Log(EnemyFSM.ActiveStateName);
    }

    public override void GetStunned(StunInfo stunInfo)
    {
        base.GetStunned(stunInfo);
        unitVisual.AbortAlert();
        agent.enabled = false;
    }

    public override void GetUnStunned()
    {
        base.GetUnStunned();
        agent.enabled = true;
    }
    
    private void RotateAttackDir()
    {
        var dirToPlayer = player.transform.position - transform.position;
        attackDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }
    
    private void FollowPlayerSensor_OnPlayerEnter(Transform player)
    {
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        isPlayerInChasingRange = true;
    }
    
    private void FollowPlayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        EnemyFSM.Trigger(StateEvent.LostPlayer);
        isPlayerInChasingRange = false;
    }

    protected bool ShouldMelee(Transition<EnemyState> transition) =>
        attackCDTimer < 0 
        && Vector2.Distance(player.transform.position, transform.position) <= 2
        && !IsStunned;
    
    protected bool IsWithinIdleRange(Transition<EnemyState> transition) => 
        !IsStunned 
        && agent.remainingDistance <= agent.stoppingDistance;

    protected bool IsNotWithinIdleRange(Transition<EnemyState> transition) => 
        !IsWithinIdleRange(transition);
    
    protected bool IsUnitStunned(Transition<EnemyState> transition) => IsStunned;

    private void OnDisable()
    {
        followPlayerSensor.OnPlayerEnter -= FollowPlayerSensor_OnPlayerEnter;
        followPlayerSensor.OnPlayerExit -= FollowPlayerSensor_OnPlayerExit;
    }
}
