using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Unit, IPickUper
{
    private IInput input;
    
    [Inject]
    private void Construct(IInput userInput, PlayerInputUI inputUI, InventoryUI playerInventoryUI, CharacterWindowUI characterWindowUI)
    {
        inputUI.Init(this, playerInventoryUI.gameObject, characterWindowUI.gameObject);
        playerInventoryUI.Init(this);
        characterWindowUI.Init(this);
        
        input = userInput;
        input.MoveInput += Move;
        input.ShootInput += Attack;

        input.ActiveAbilityInput += ExecuteActiveAbility;
    }

    protected override void Death(Unit killer)
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnDisable()
    {
        input.MoveInput -= Move;
        input.ShootInput -= Attack;

        input.ActiveAbilityInput -= ExecuteActiveAbility;
    }

    protected override void Update()
    {
        base.Update();
        RotateAttackDir();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, GetWeapon().AttackDistance + 0.5f);
    }

    public void GetExp(int exp)
    {
        Stats.GetExp(exp);
    }

    private void RotateAttackDir()
    {
        attackDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }

    public int TryPickUpItem(Item item, int amount)
    {
        return Inventory.TryAddItem(item, amount);
    }
}


