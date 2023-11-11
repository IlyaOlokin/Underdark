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
    private Transform player;

    [Header("Enemy Setup")] 
    [SerializeField] private Transform spawnPont;
    [SerializeField] protected Transform moveTarget;
    [SerializeField] protected NavMeshAgent agent;
    protected StateMachine<EnemyState, StateEvent> EnemyFSM;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    [SerializeField] protected Collider2D allySensor;
    [SerializeField] protected LayerMask alliesLayer;

    public bool CanMove => !IsStunned && !IsPushing;
    
    [SerializeField] private int expPerLevel;
    
    protected bool isPlayerInMeleeRange;
    protected bool isPlayerInChasingRange;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        moveTarget.transform.SetParent(transform.parent);
        
        EnemyFSM = new();
    }

    private void OnEnable()
    {
        followPlayerSensor.OnPlayerEnter += FollowPlayerSensor_OnPlayerEnter;
        followPlayerSensor.OnPlayerExit += FollowPlayerSensor_OnPlayerExit;
        EnemyFSM.RequestStateChange(EnemyState.Idle, true);
        SetUnit();
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

    public override bool TakeDamage(Unit sender, IAttacker attacker, float damage, bool evadable = true, float armorPierce = 0f)
    {
        var res = base.TakeDamage(sender, attacker, damage, evadable);
        Agr(sender.transform.position);
        AgrNearbyAllies();

        return res;
    }

    public void Agr(Vector3 pos)
    {
        moveTarget.position = pos;
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
    }

    private void AgrNearbyAllies()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(alliesLayer);
        List<Collider2D> hitColliders = new List<Collider2D>();

        allySensor.OverlapCollider(contactFilter, hitColliders);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out Enemy enemy))
            {
                if (enemy != this)
                    enemy.Agr(moveTarget.position);
            }
        }
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
        if (TryGetComponent(out Drop drop))
            drop.DropItems();
        
        if (killer.TryGetComponent(out Player player))
            player.GetExp(Stats.Level * expPerLevel);
        
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
        var dirToPlayer = moveTarget.transform.position - transform.position;
        attackDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }
    
    private void FollowPlayerSensor_OnPlayerEnter(Transform player)
    {
        moveTarget.position = player.position;
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        isPlayerInChasingRange = true;
        this.player = player;
        AgrNearbyAllies();
    }
    
    private void FollowPlayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        moveTarget.position = lastKnownPosition;
        isPlayerInChasingRange = false;
    }

    protected bool ShouldMelee(Transition<EnemyState> transition) =>
        attackCDTimer < 0 
        && isPlayerInChasingRange
        && Vector2.Distance(moveTarget.transform.position, transform.position) <= GetWeapon().AttackDistance + 1
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
