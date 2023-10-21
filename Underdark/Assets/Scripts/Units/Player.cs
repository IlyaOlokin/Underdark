using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Unit
{
    private IInput input;
    
    [Inject]
    private void Construct(IInput userInput, PlayerInputUI inputUI)
    {
        inputUI.player = this;
        input = userInput;
        input.MoveInput += Move;
        input.ShootInput += Attack;

        input.ActiveAbilityInput += ExecuteActiveAbility;
        Inventory = new Inventory(10);
        Inventory.TryAddItem(Weapon);
    }

    protected override void Death()
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var slot in Inventory.GetAllSlots())
            {
                if (slot.Item == null) Debug.Log("Empty");
                else Debug.Log(slot.Item.ID);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Weapon.AttackDistance + 0.5f);
    }

    private void RotateAttackDir()
    {
        attackDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }
}


