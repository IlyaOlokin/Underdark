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

public class NPCUnit : Unit
{
    protected Transform targetUnit;

    [Header("NPC Setup")] 
    [SerializeField] private Transform spawnPont;
    [SerializeField] protected Transform moveTarget;
    [SerializeField] protected NavMeshAgent agent;
    protected StateMachine<NPCState, StateEvent> NPCFSM;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    [SerializeField] protected Collider2D allySensor;
    [FormerlySerializedAs("lostPlayerDelay")] [SerializeField] private float lostTargetDelay;
    [SerializeField] private float searchTargetDelay = 0.5f;
    private float lostTargetTimer;
    private const float AgrRadius = 20;
    
    private int PreparedActiveAbilityIndex { get; set; }
    
    [SerializeField] protected float meleeAttackDuration;
    [SerializeField] protected float meleeAttackPreparation;

    public bool CanMove => !IsDisabled && !IsPushing;
    
    [SerializeField] private int expPerLevel;
    
    protected bool isPlayerInMeleeRange;
    protected bool isPlayerInChasingRange => targetUnit != null;
    
    [Header("Summon Setup")]
    private bool isSummon;
    private bool isGoingToSpawnPoint;
    [SerializeField] private float toFarFromSpawnPointRadius = 10f;
    
    [Header("Range NPC Setup")] 
    [SerializeField] private float distToForceMelee;
    [SerializeField] private float distToKeep;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = MoveSpeed;
        moveTarget.transform.SetParent(transform.parent);
        
        NPCFSM = new();
    }

    private void OnEnable()
    {
        followPlayerSensor.OnTargetEnter += FindClosestTargetUnit;
        followPlayerSensor.OnTargetExit += FollowTargetSensorOnTargetExit;
        NPCFSM.RequestStateChange(NPCState.Idle, true);
        SetUnit();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SearchForTargetUnits());
    }

    public void SetSummonedUnit(Transform spawnPoint, string tag, int layer, LayerMask enemyLayerMask, LayerMask alliesLayerMask)
    {
        isSummon = true;
        spawnPont = spawnPoint;
        
        AlliesLayer = alliesLayerMask;
        
        followPlayerSensor.SetLayerMask(enemyLayerMask);
        AttackMask = enemyLayerMask;
        
        transform.tag = tag;
        gameObject.layer = layer;
    }

    protected override void Update()
    {
        base.Update();
        NPCFSM.OnLogic();
        TryFlipVisual(agent.velocity.x);
        if (isPlayerInChasingRange)
            lastMoveDir = targetUnit.position - transform.position;
        else 
            TryToReturnToSpawnPoint();
        
        if (isSummon)
            HandleSummonReturn();
    }

    private void HandleSummonReturn()
    {
        var distanceToSpawnPoint = Vector2.Distance(spawnPont.position, transform.position);
        if (!isGoingToSpawnPoint && distanceToSpawnPoint > toFarFromSpawnPointRadius)
        {
            isGoingToSpawnPoint = true;
            moveTarget.position = spawnPont.position;
            targetUnit = null;
            NPCFSM.Trigger(StateEvent.StartChase);
        }
        else if (isGoingToSpawnPoint && distanceToSpawnPoint < toFarFromSpawnPointRadius / 2f)
        {
            isGoingToSpawnPoint = false;
        }
        if (!isPlayerInChasingRange)
            moveTarget.position = spawnPont.position;
    }

    private void TryToReturnToSpawnPoint()
    {
        if (DistToTargetPos() < agent.stoppingDistance && Vector2.Distance(spawnPont.position, transform.position) > agent.stoppingDistance)
        {
            lostTargetTimer -= Time.deltaTime;
            if (lostTargetTimer <= 0)
            {
                moveTarget.position = spawnPont.position;
                NPCFSM.Trigger(StateEvent.StartChase);
            }
        }
    }

    protected void ChaseTarget()
    {
        if (isPlayerInChasingRange)
            if (ShouldKeepDistance(null))
                moveTarget.position = targetUnit.transform.position + (transform.position - targetUnit.transform.position).normalized * distToKeep;
            else
                moveTarget.position = targetUnit.transform.position;
    }

    public override bool TakeDamage(Unit sender, IAttacker attacker, DamageInfo damageInfo, bool evadable = true, float armorPierce = 0f)
    {
        var res = base.TakeDamage(sender, attacker, damageInfo, evadable);
        if (damageInfo.MustAggro && Vector2.Distance(transform.position, sender.transform.position) < AgrRadius)
        {
            Agr(sender.transform.position);
            AgrNearbyAllies();
        }

        return res;
    }

    public void Agr(Vector3 pos)
    {
        moveTarget.position = pos;
        NPCFSM.Trigger(StateEvent.StartChase);
    }

    private void AgrNearbyAllies()
    {
        var hitColliders = GetNearbyAllies();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.TryGetComponent(out NPCUnit enemy))
            {
                if (enemy != this)
                    enemy.Agr(moveTarget.position);
            }
        }
    }

    public List<Collider2D> GetNearbyAllies()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(AlliesLayer);
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
        if (!isSummon && TryGetComponent(out Drop drop))
        {
            if (killer.TryGetComponent(out IMoneyHolder moneyHolder)) drop.DropItems(moneyHolder);
            else drop.DropItems();
        }
        
        if (killer.TryGetComponent(out Player player))
            player.Stats.GetExp(Stats.Level * expPerLevel);
        
        UnitVisual.StartDeathEffect(attacker, damageType);
        
        base.Death(killer, attacker, damageType);
        if (isSummon) Destroy(gameObject);
    }
    public override void ApplySlowDebuff(float slow)
    {
        base.ApplySlowDebuff(slow);
        agent.speed = MoveSpeed * Params.SlowDebuffAmount * Params.MoveSpeedMultiplier;
    }
    
    public override void GetStunned()
    {
        base.GetStunned();
        UnitVisual.AbortAlert();
        UpdateMovementAbility();
        
    }

    public override void GetUnStunned()
    {
        base.GetUnStunned();
        UpdateMovementAbility();
    }
    
    public override void GetFrozen()
    {
        base.GetFrozen();
        UnitVisual.AbortAlert();
        UpdateMovementAbility();
    }

    public override void GetUnFrozen()
    {
        base.GetUnFrozen();
        UpdateMovementAbility();
    }

    public override void GetPushed(Vector2 pushDir)
    {
        base.GetPushed(pushDir);
        UpdateMovementAbility();
    }
    
    public override void EndPushState()
    {
        base.EndPushState();
        UpdateMovementAbility();
    }
    protected override void RotateAttackDir()
    {
        var dirToPlayer = isPlayerInChasingRange
            ? targetUnit.transform.position - transform.position
            : moveTarget.transform.position - transform.position;

        lastMoveDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) lastMoveDirAngle *= -1;
        unitVisualRotatable.transform.eulerAngles = new Vector3(0, 0, lastMoveDirAngle - 90);
    }

    protected void ExecuteActiveAbility()
    {
        ExecuteActiveAbility(PreparedActiveAbilityIndex);
    }

    private IEnumerator SearchForTargetUnits()
    {
        FindClosestTargetUnit(null);
        yield return new WaitForSeconds(searchTargetDelay);
        StartCoroutine(SearchForTargetUnits());
    }

    private void FindClosestTargetUnit(Transform newTarget)
    {
        if (isGoingToSpawnPoint) return;
        
        var minDist = float.MaxValue;
        Transform currTarget = newTarget;

        for (var i = followPlayerSensor.Targets.Count - 1; i >= 0; i--)
        {
            var target = followPlayerSensor.Targets[i];
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                followPlayerSensor.Targets.Remove(target);
                continue;
            }
            var currDist = Vector2.Distance(target.position, transform.position);
            if (currDist < minDist)
            {
                minDist = currDist;
                currTarget = target;
            }
        }

        targetUnit = currTarget;
        if (targetUnit == null) return;
        
        moveTarget.position = currTarget.position;
        NPCFSM.Trigger(StateEvent.StartChase);
        lostTargetTimer = lostTargetDelay;
        AgrNearbyAllies();
    }
    
    private void FollowTargetSensorOnTargetExit(Transform target)
    {
        if (target != targetUnit) return;
        moveTarget.position = target.position;
        targetUnit = null;
        NPCFSM.Trigger(StateEvent.StartChase);
    }

    protected bool ShouldMelee(Transition<NPCState> transition) =>
        attackCDTimer < 0
        && isPlayerInChasingRange
        && DistToTargetPos() <= GetWeapon().AttackDistance + 0.7f
        && !IsDisabled
        && Physics2D
            .Raycast(transform.position,
                this.targetUnit.transform.position - transform.position, 
                Mathf.Infinity,
                AttackMask)
            .collider.TryGetComponent(out Unit target);
    
    protected bool ShouldUseActiveAbility(Transition<NPCState> transition)
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
    
    private bool IsCloseEnoughToForceMelee(Transition<NPCState> transition)
    {
        return isPlayerInChasingRange && DistToTargetPos() < distToForceMelee;
    }
    
    private bool ShouldKeepDistance(Transition<NPCState> transition)
    {
        return !IsCloseEnoughToForceMelee(transition)
               && (CanUseActiveAbility(transition, 0)
                   || CanUseActiveAbility(transition, 1)
                   || CanUseActiveAbility(transition, 2)
                   || CanUseActiveAbility(transition, 3));
    }

    protected bool CanUseActiveAbility(Transition<NPCState> transition, int index) =>
        isPlayerInChasingRange
        && !Inventory.EquippedActiveAbilitySlots[index].IsEmpty
        && ((ActiveAbilitySO)Inventory.EquippedActiveAbilitySlots[index].Item).ActiveAbility.CanUseAbility(this, DistToTargetPos())
        && !IsDisabled
        && !IsSilenced
        && Physics2D
            .Raycast(transform.position,
                this.targetUnit.transform.position - transform.position,
                Mathf.Infinity,
                AttackMask)
            .collider.TryGetComponent(out Unit target);

    protected bool CanNotAnyUseActiveAbility(Transition<NPCState> transition) => 
        isPlayerInChasingRange 
        && !CanUseActiveAbility(transition, 0)
        && !CanUseActiveAbility(transition, 1)
        && !CanUseActiveAbility(transition, 2)
        && !CanUseActiveAbility(transition, 3);

    protected bool ShouldAttack(Transition<NPCState> transition) =>
        ShouldMelee(transition) || ShouldUseActiveAbility(transition);
    
    protected bool IsWithinIdleRange(Transition<NPCState> transition) => 
        CanMove
        && !isPlayerInChasingRange
        && agent.remainingDistance <= agent.stoppingDistance;

    protected bool IsNotWithinIdleRange(Transition<NPCState> transition) => 
        !IsWithinIdleRange(transition);
    
    protected bool IsUnitStunned(Transition<NPCState> transition) => IsDisabled;

    private void OnDisable()
    {
        followPlayerSensor.OnTargetEnter -= FindClosestTargetUnit;
        followPlayerSensor.OnTargetExit -= FollowTargetSensorOnTargetExit;
    }

    protected float DistToMovePos()
    {
        return Vector2.Distance(moveTarget.transform.position, transform.position);
    }
    
    protected float DistToTargetPos()
    {
        if (!isPlayerInChasingRange) return DistToMovePos();
        return Vector2.Distance(targetUnit.transform.position, transform.position);
    }
}