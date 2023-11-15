using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Player : Unit, IPickUper
{
    private IInput input;
    public event Action OnExpGained;
    
    [Inject]
    private void Construct(IInput userInput, PlayerInputUI inputUI, PlayerUI playerUI)
    {
        inputUI.Init(this, playerUI.inventoryUI.gameObject, playerUI.characterWindowUI.gameObject);
        playerUI.Init(this);

        input = userInput;
        input.MoveInput += Move;
        input.ShootInput += Attack;

        input.ActiveAbilityInput += ExecuteActiveAbility;
        input.ActiveAbilityHoldStart += StartHighLightActiveAbility;
        input.ActiveAbilityHoldEnd += EndHighLightActiveAbility;

        input.ExecutableItemInput += ExecuteExecutableItem;
    }
    
    private void OnEnable()
    {
        Inventory.OnEquipmentChanged += SetAttackCollider;
        Inventory.OnActiveAbilitiesChanged += SetActiveAbilitiesCDs;
        Stats.OnStatsChanged += SetHP;
        Stats.OnStatsChanged += SetMana;
        Stats.OnLevelUp += OnLevelUp;
    }

    protected override void Death(Unit killer)
    {
        base.Death(killer);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    protected override void Update()
    {
        base.Update();
        RotateAttackDir();

        if (Input.GetKeyDown(KeyCode.R)) // debug
        {
            GetEnergyShield(9, 180);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, GetWeapon().AttackDistance + 0.5f);
    }
    
    private void StartHighLightActiveAbility(int index)
    {  
        if (Inventory.EquippedActiveAbilitySlots[index].IsEmpty) return;
        var dist = ((ActiveAbilitySO)Inventory.EquippedActiveAbilitySlots[index].Item).ActiveAbility.AttackDistance;
        var angle = ((ActiveAbilitySO)Inventory.EquippedActiveAbilitySlots[index].Item).ActiveAbility.AttackAngle;

        unitVisual.StartHighLightActiveAbility(dist, angle);
    }
    
    private void EndHighLightActiveAbility(int index)
    {
        unitVisual.EndHighLightActiveAbility();
    }

    public void GetExp(int exp)
    {
        Stats.GetExp(exp);
        OnExpGained?.Invoke();
    }

    private void RotateAttackDir()
    {
        attackDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
        unitVisualRotatable.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }

    public int TryPickUpItem(Item item, int amount)
    {
        return Inventory.TryAddItem(item, amount);
    }
    
    private void OnDisable()
    {
        input.MoveInput -= Move;
        input.ShootInput -= Attack;

        input.ActiveAbilityInput -= ExecuteActiveAbility;
        
        input.ExecutableItemInput -= ExecuteExecutableItem;
        
        Inventory.OnEquipmentChanged -= SetAttackCollider;
        Inventory.OnActiveAbilitiesChanged -= SetActiveAbilitiesCDs;
        Stats.OnStatsChanged -= SetHP;
        Stats.OnStatsChanged -= SetMana;
        Stats.OnLevelUp -= OnLevelUp;
    }
}


