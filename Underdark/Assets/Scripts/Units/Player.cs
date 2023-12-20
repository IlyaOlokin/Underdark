using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Player : Unit, IPickUper, IMoneyHolder
{
    private IInput input;
    public Money Money { get; private set; }
    
    [Inject]
    private void Construct(IInput userInput, PlayerInputUI inputUI, PlayerUI playerUI)
    {
        inputUI.Init(this, userInput, playerUI.gearUI.gameObject, playerUI.characterWindowUI.gameObject, playerUI.menuWindowUI);
        playerUI.Init(this);

        input = userInput;
        input.MoveInput += Move;
        input.ShootInput += Attack;

        input.ActiveAbilityInput += ExecuteActiveAbility;
        input.ActiveAbilityHoldStart += StartHighLightActiveAbility;
        input.ActiveAbilityHoldEnd += EndHighLightActiveAbility;

        input.ExecutableItemInput += ExecuteExecutableItem;

        Money = new Money();
    }
    
    private void OnEnable()
    {
        Inventory.OnActiveAbilitiesChanged += SetActiveAbilitiesCDs;
        Stats.OnStatsChanged += UpdateHP;
        Stats.OnStatsChanged += UpdateMP;
        Stats.OnLevelUp += OnLevelUp;
    }
    
    protected override void Update()
    {
        base.Update();
        RotateAttackDir();

        if (Input.GetKeyDown(KeyCode.R)) // debug
        {
            DataLoader.SaveGame(this);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, GetWeapon().AttackDistance + 0.5f);
    }
    
    private void StartHighLightActiveAbility(int index)
    {  
        if (Inventory.EquippedActiveAbilitySlots[index].IsEmpty) return;
        var activeAbility = ((ActiveAbilitySO)Inventory.EquippedActiveAbilitySlots[index].Item).ActiveAbility;
        
        unitVisual.StartHighLightActiveAbility(activeAbility, GetWeapon());
    }
    
    private void EndHighLightActiveAbility(int index)
    {
        unitVisual.EndHighLightActiveAbility();
    }

    protected override void Death(Unit killer, IAttacker attacker, DamageType damageType)
    {
        base.Death(killer, attacker, damageType);
        Stats.LooseXP();
        DataLoader.SaveGame(this);
    }
    
    private void RotateAttackDir()
    {
        lastMoveDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) lastMoveDirAngle *= -1;
        unitVisualRotatable.transform.eulerAngles = new Vector3(0, 0, lastMoveDirAngle - 90);
    }

    public int TryPickUpItem(Item item, int amount)
    {
        return Inventory.TryAddItem(item, amount);
    }
    
    public override Vector2 GetAttackDirection(float distance = 0)
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(AttackMask);
        List<Collider2D> hitColliders = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, distance + 0.5f, contactFilter, hitColliders);

        Collider2D target = null;
        float minDist = float.MaxValue;
        foreach (var collider in hitColliders)
        {
            if (!ActiveAbility.HitCheck(transform, collider.transform, contactFilter)) continue;

            var distToTarget = Vector3.Distance(transform.position, collider.transform.position);
            if (distToTarget < minDist)
            {
                minDist = distToTarget;
                target = collider;
            }
        }

        return target == null ? lastMoveDir.normalized : (target.transform.position - transform.position).normalized;
    }
    
    public override float GetAttackDirAngle(Vector2 attackDir = new Vector2())
    { 
        var angle = Vector3.Angle(Vector3.right, attackDir);
        if (attackDir.y < 0) angle *= -1;
        return angle;
    }
    
    private void OnDisable()
    {
        input.MoveInput -= Move;
        input.ShootInput -= Attack;

        input.ActiveAbilityInput -= ExecuteActiveAbility;
        
        input.ExecutableItemInput -= ExecuteExecutableItem;
        
        Inventory.OnActiveAbilitiesChanged -= SetActiveAbilitiesCDs;
        Stats.OnStatsChanged -= UpdateHP;
        Stats.OnStatsChanged -= UpdateMP;
        Stats.OnLevelUp -= OnLevelUp;
    }
}