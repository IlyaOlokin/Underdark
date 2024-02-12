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
    protected Transform player;

    [Header("Enemy Setup")] 
    [SerializeField] private Transform spawnPont;
    [SerializeField] protected Transform moveTarget;
    [SerializeField] protected NavMeshAgent agent;
    protected StateMachine<EnemyState, StateEvent> EnemyFSM;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    [SerializeField] protected Collider2D allySensor;
    [SerializeField] protected LayerMask alliesLayer;
    [SerializeField] private float lostPlayerDelay;
    private float lostPlayerTimer;
    public int PreparedActiveAbilityIndex { get; private set; }
    
    [SerializeField] protected float meleeAttackDuration;
    [SerializeField] protected float meleeAttackPreparation;

    public bool CanMove => !IsDisabled && !IsPushing;
    
    [SerializeField] private int expPerLevel;
    
    protected bool isPlayerInMeleeRange;
    protected bool isPlayerInChasingRange;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = MoveSpeed;
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
        TryFlipVisual(agent.velocity.x);
        if (isPlayerInChasingRange)
            lastMoveDir = player.position - transform.position;
        else 
            TryToReturnToSpawnPoint();
    }

    protected void TryToReturnToSpawnPoint()
    {
        if (DistToTargetPos() < agent.stoppingDistance)
        {
            lostPlayerTimer -= Time.deltaTime;
            if (lostPlayerTimer <= 0)
            {
                moveTarget.position = spawnPont.position;
                EnemyFSM.Trigger(StateEvent.StartChase);
            }
        }
    }

    protected void ChaseTarget()
    {
        if (isPlayerInChasingRange)
            moveTarget.position = player.transform.position;
    }

    public override bool TakeDamage(Unit sender, IAttacker attacker, DamageInfo damageInfo, bool evadable = true, float armorPierce = 0f)
    {
        Agr(sender.transform.position); // optional
        var res = base.TakeDamage(sender, attacker, damageInfo, evadable);
        AgrNearbyAllies(); // optional

        return res;
    }

    public void Agr(Vector3 pos)
    {
        moveTarget.position = pos;
        EnemyFSM.Trigger(StateEvent.StartChase);
    }

    private void AgrNearbyAllies()
    {
        var hitColliders = GetNearbyAllies();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out Enemy enemy))
            {
                if (enemy != this)
                    enemy.Agr(moveTarget.position);
            }
        }
    }

    public List<Collider2D> GetNearbyAllies()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(alliesLayer);
        List<Collider2D> hitColliders = new List<Collider2D>();

        allySensor.OverlapCollider(contactFilter, hitColliders);
        return hitColliders;
    }

    private void UpdateMovementAbility()
    {
        if (!IsDisabled && !IsPushing)
            agent.enabled = true;
        else
            agent.enabled = false;
    }

    protected override void Death(Unit killer, IAttacker attacker, DamageType damageType)
    {
        if (TryGetComponent(out Drop drop))
        {
            if (killer.TryGetComponent(out IMoneyHolder moneyHolder)) drop.DropItems(moneyHolder);
            else drop.DropItems();
        }
        
        if (killer.TryGetComponent(out Player player))
            player.Stats.GetExp(Stats.Level * expPerLevel);
        
        unitVisual.StartDeathEffect(attacker, damageType);
        
        base.Death(killer, attacker, damageType);
    }
    public override void ApplySlow(float slow)
    {
        base.ApplySlow(slow);
        agent.speed = MoveSpeed / slow;
    }
    
    public override bool GetStunned(StunInfo stunInfo, Sprite effectIcon)
    {
        if (!base.GetStunned(stunInfo, effectIcon)) return false;
        
        unitVisual.AbortAlert();
        UpdateMovementAbility();
        return true;
    }

    public override void GetUnStunned()
    {
        base.GetUnStunned();
        UpdateMovementAbility();
    }
    
    public override bool GetFrozen(FreezeInfo freezeInfo, Sprite effectIcon)
    {
        if (!base.GetFrozen(freezeInfo, effectIcon)) return false;
        
        unitVisual.AbortAlert();
        UpdateMovementAbility();
        return true;
    }

    public override void GetUnFrozen()
    {
        base.GetUnFrozen();
        UpdateMovementAbility();
    }

    public override bool GetPushed(PushInfo pushInfo, Vector2 pushDir, Sprite effectIcon)
    {
        if (!base.GetPushed(pushInfo, pushDir, effectIcon)) return false;
        UpdateMovementAbility();
        return true;
    }
    
    public override void EndPush()
    {
        base.EndPush();
        UpdateMovementAbility();
    }
    protected override void RotateAttackDir()
    {
        var dirToPlayer = isPlayerInChasingRange
            ? player.transform.position - transform.position
            : moveTarget.transform.position - transform.position;

        lastMoveDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) lastMoveDirAngle *= -1;
        unitVisualRotatable.transform.eulerAngles = new Vector3(0, 0, lastMoveDirAngle - 90);
    }

    protected void ExecuteActiveAbility()
    {
        ExecuteActiveAbility(PreparedActiveAbilityIndex);
    }
    
    private void FollowPlayerSensor_OnPlayerEnter(Transform player)
    {
        moveTarget.position = player.position;
        EnemyFSM.Trigger(StateEvent.StartChase);
        isPlayerInChasingRange = true;
        this.player = player;
        lostPlayerTimer = lostPlayerDelay;
        AgrNearbyAllies();
    }
    
    private void FollowPlayerSensor_OnPlayerExit(Vector3 lastKnownPosition)
    {
        moveTarget.position = lastKnownPosition;
        isPlayerInChasingRange = false;
        player = null;
    }

    protected bool ShouldMelee(Transition<EnemyState> transition) =>
        attackCDTimer < 0
        && isPlayerInChasingRange
        && DistToTargetPos() <= GetWeapon().AttackDistance + 0.7f
        && !IsDisabled
        && Physics2D
            .Raycast(transform.position,
                this.player.transform.position - transform.position, 
                Mathf.Infinity,
                AttackMask)
            .collider.TryGetComponent(out Player player);
    
    protected bool ShouldUseActiveAbility(Transition<EnemyState> transition)
    {
        for (int i = 0; i < Inventory.EquippedActiveAbilitySlots.Count; i++)
        {
            if (ActiveAbilitiesCD[i] < 0 && CanUseActiveAbility(transition, i))
            {
                PreparedActiveAbilityIndex = i;
                return true;
            }
        }

        return false;
    }

    protected bool CanUseActiveAbility(Transition<EnemyState> transition, int index) =>
        isPlayerInChasingRange
        && !Inventory.EquippedActiveAbilitySlots[index].IsEmpty
        && ((ActiveAbilitySO)Inventory.EquippedActiveAbilitySlots[index].Item).ActiveAbility.CanUseAbility(this, DistToTargetPos())
        && !IsDisabled
        && !IsSilenced
        && Physics2D
            .Raycast(transform.position,
                this.player.transform.position - transform.position,
                Mathf.Infinity,
                AttackMask)
            .collider.TryGetComponent(out Player player);

    protected bool CanNotAnyUseActiveAbility(Transition<EnemyState> transition) => 
        isPlayerInChasingRange 
        && !CanUseActiveAbility(transition, 0)
        && !CanUseActiveAbility(transition, 1)
        && !CanUseActiveAbility(transition, 2)
        && !CanUseActiveAbility(transition, 3);

    protected bool ShouldAttack(Transition<EnemyState> transition) =>
        ShouldMelee(transition) || ShouldUseActiveAbility(transition);
    
    protected bool IsWithinIdleRange(Transition<EnemyState> transition) => 
        CanMove
        && !isPlayerInChasingRange
        && agent.remainingDistance <= agent.stoppingDistance;

    protected bool IsNotWithinIdleRange(Transition<EnemyState> transition) => 
        !IsWithinIdleRange(transition);
    
    protected bool IsUnitStunned(Transition<EnemyState> transition) => IsDisabled;

    private void OnDisable()
    {
        followPlayerSensor.OnPlayerEnter -= FollowPlayerSensor_OnPlayerEnter;
        followPlayerSensor.OnPlayerExit -= FollowPlayerSensor_OnPlayerExit;
    }

    protected float DistToMovePos()
    {
        return Vector2.Distance(moveTarget.transform.position, transform.position);
    }
    
    protected float DistToTargetPos()
    {
        if (!isPlayerInChasingRange) return DistToMovePos();
        return Vector2.Distance(player.transform.position, transform.position);
    }
}