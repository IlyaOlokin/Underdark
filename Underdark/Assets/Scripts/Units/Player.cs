using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Unit
{
    //[SerializeField] private EnemySeeker enemySeeker;
    [SerializeField] private Transform HandGun;
    
    private IInput input;

    
    //private Inventory inventory;
    
    [Inject]
    private void Construct(IInput userInput)
    {
        input = userInput;
        input.MoveInput += Move;
        input.ShootInput += Attack;
    }

    protected override void Death()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void OnDisable()
    {
        input.MoveInput -= Move;
    }

    protected override void Update()
    {
        base.Update();
        RotateAttackDir();
    }

    private void RotateAttackDir()
    {
        attackDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }
}


