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
        //this.inventory = inventory;
    }

    /*protected void Update()
    {
        if (enemySeeker.ClosestTarget == null) return;
        
        PointGunAtTarget();
    }

    private void PointGunAtTarget()
    {
        Vector3 dir = transform.position - enemySeeker.ClosestTarget.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        HandGun.transform.eulerAngles = new Vector3(0, 0, angle + 180);
    }*/

    protected override void Death()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*public bool PickItem(ItemType item)
    {
        return inventory.TryPutItemIn(item, 1);
    }*/

    public override void Move(Vector3 dir)
    {
        dir = dir.normalized;
        //rb.velocity = new Vector2(dir.x * MoveSpeed, dir.y * MoveSpeed);
        rb.MovePosition(rb.position + (Vector2) dir * MoveSpeed * Time.fixedDeltaTime);
        TryFlipVisual(dir.x);
    }

    private void OnDisable()
    {
        input.MoveInput -= Move;
    }
    
    public override void Attack()
    {
        /*if (enemySeeker.ClosestTarget == null) return;
        if (inventory.BulletCount <= 0) return;

        Vector3 dir = enemySeeker.ClosestTarget.position - transform.position;
        GameObject attack = Instantiate(AttackPrefab, transform.position, Quaternion.identity);
        var component = attack.GetComponent<Bullet>();
        component.Damage = Damage;
        component.Dir = dir;
        inventory.DecreaseBulletCount(1);*/
    }
}


