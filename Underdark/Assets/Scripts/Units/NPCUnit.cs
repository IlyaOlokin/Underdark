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
using Timer = Unity.VisualScripting.Timer;

public class NPCUnit : Unit
{
    protected Transform targetUnit;
    protected StateMachine<NPCState, StateEvent> NPCFSM;

    [Header("NPC Setup")] 
    [SerializeField] private Transform spawnPont;
    [SerializeField] protected Transform moveTarget;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected PlayerSensor followPlayerSensor;
    [SerializeField] protected Collider2D allySensor;
    [FormerlySerializedAs("lostPlayerDelay")] 
    [SerializeField] private float lostTargetDelay;
    [SerializeField] private float searchTargetDelay = 0.5f;
    private float lostTargetTimer;
    private const float AgrRadius = 20;
    
    private int PreparedActiveAbilityIndex { get; set; }
    
    [SerializeField] protected float meleeAttackDuration;
    [SerializeField] protected float meleeAttackPreparation;
    [SerializeField] private int expPerLevel;
    
    protected bool hasTarget => targetUnit != null;
    public bool CanMove => !IsDisabled && !IsPushing;

    private float flipDelay = 0.1f;
    private float flipTimer;
    
    [Header("Summon Setup")]
    private bool isSummon;
    private bool isGoingToSpawnPoint;
    private Unit caster;
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
        moveTarget.position = spawnPont.position;
        UnitVisual.AbortAlert();
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(SearchForTargetUnits());
    }

    public void SetSummonedUnit(Unit caster, Transform spawnPoint, string tag, int layer, LayerMask enemyLayerMask, LayerMask alliesLayerMask)
    {
        this.caster = caster;
        
        isSummon = true;
        spawnPont = spawnPoint;
        moveTarget.position = spawnPont.position;
        
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

        if (flipTimer > 0) flipTimer -= Time.deltaTime;
        else TryFlipVisual(agent.velocity.x);
        
        if (hasTarget)
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
        if (!hasTarget && DistToMovePos() < agent.stoppingDistance)
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
    
    protected override bool TryFlipVisual(float moveDir)
    {
        var flipped = base.TryFlipVisual(moveDir);
        if (flipped) flipTimer = flipDelay;
        return flipped;
    }

    protected void ChaseTarget()
    {
        if (hasTarget)
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
        if (isGoingToSpawnPoint) return;
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
        
        killer.GetExp(Stats.Level * expPerLevel);

        targetUnit = null;
        
        UnitVisual.StartDeathEffect(attacker, damageType);
        
        base.Death(killer, attacker, damageType);
        if (isSummon) Destroy(gameObject);
    }

    public override void GetExp(int exp)
    {
        if (!isSummon) return;
        
        if (caster != null)
            caster.GetExp(exp);
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
        var dirToPlayer = hasTarget
            ? targetUnit.transform.position - transform.position
            : moveTarget.transform.position - transform.position;

        lastMoveDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) lastMoveDirAngle *= -1;
        UnitVisualRotatable.transform.eulerAngles = new Vector3(0, 0, lastMoveDirAngle - 90);
    }

    protected void ExecuteActiveAbility()
    {
        ExecuteActiveAbility(PreparedActiveAbilityIndex);
    }

    private IEnumerator SearchForTargetUnits()
    {
        while (true)
        {
            FindClosestTargetUnit(null);
            yield return new WaitForSeconds(searchTargetDelay);
        }
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
        && hasTarget
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
        return hasTarget && DistToTargetPos() < distToForceMelee;
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
        hasTarget
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
        hasTarget 
        && !CanUseActiveAbility(transition, 0)
        && !CanUseActiveAbility(transition, 1)
        && !CanUseActiveAbility(transition, 2)
        && !CanUseActiveAbility(transition, 3);

    protected bool ShouldAttack(Transition<NPCState> transition) =>
        ShouldMelee(transition) || ShouldUseActiveAbility(transition);
    
    protected bool IsWithinIdleRange(Transition<NPCState> transition) => 
        CanMove
        && !hasTarget
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
        if (!hasTarget) return DistToMovePos();
        return Vector2.Distance(targetUnit.transform.position, transform.position);
    }
}