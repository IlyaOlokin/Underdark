using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityHFSM;
using Zenject;
using Random = UnityEngine.Random;

public class Enemy : Unit
{
    protected Player player;
    
    [Header("Enemy Setup")] 
    [SerializeField] protected Transform moveTarget;
    [SerializeField] protected NavMeshAgent agent;
    protected StateMachine<EnemyState, StateEvent> EnemyFSM;
    [SerializeField] protected PlayerSensor followPlayerSensor;

    public bool CanMove => !IsStunned && !IsPushing;

    [Header("Drop")]
    [SerializeField] private DroppedItem droppedItemPref;
    [SerializeField] private List<ItemToDrop> drop;
    [SerializeField] private int expPerLevel;
    
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
        moveTarget.transform.SetParent(transform.parent);
        
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
        if (isPlayerInChasingRange)
            moveTarget.position = player.transform.position;
    }

    public override bool TakeDamage(Unit sender, float damage, bool evadable = true)
    {
        var res = base.TakeDamage(sender, damage, evadable);
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        moveTarget.position = sender.transform.position;
        
        return res;
    }

    private void UpdateMovementAbility()
    {
        if (!IsStunned && !IsPushing)
            agent.enabled = true;
        else
            agent.enabled = false;
    }

    protected override void Death(Unit killer)
    {
        foreach (var itemToDrop in drop)
        {
            if (Random.Range(0f, 1f) < itemToDrop.ChanceToDrop)
            {
                var newDrop = Instantiate(droppedItemPref, transform.position, Quaternion.identity);
                newDrop.SetDroppedItem(itemToDrop.Item, itemToDrop.ItemAmount);
            }
        }

        if (killer.TryGetComponent(out Player player))
        {
            player.GetExp(Stats.Level * expPerLevel);
        }
        
        base.Death(killer);
    }

    public override bool GetStunned(StunInfo stunInfo)
    {
        if (!base.GetStunned(stunInfo)) return false;
        
        unitVisual.AbortAlert();
        UpdateMovementAbility();
        return true;
    }

    public override void GetUnStunned()
    {
        base.GetUnStunned();
        UpdateMovementAbility();
    }

    public override bool GetPushed(PushInfo pushInfo, Vector2 pushDir)
    {
        if (!base.GetPushed(pushInfo, pushDir)) return false;
        UpdateMovementAbility();
        return true;
    }
    
    public override void EndPush()
    {
        base.EndPush();
        UpdateMovementAbility();
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
        moveTarget.position = lastKnownPosition;
        isPlayerInChasingRange = false;
    }

    protected bool ShouldMelee(Transition<EnemyState> transition) =>
        attackCDTimer < 0 
        && Vector2.Distance(player.transform.position, transform.position) <= 2
        && !IsStunned;
    
    protected bool IsWithinIdleRange(Transition<EnemyState> transition) => 
        CanMove
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
