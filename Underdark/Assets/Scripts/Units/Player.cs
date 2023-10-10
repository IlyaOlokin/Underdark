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

    [SerializeField] private LayerMask attackMask;
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
    
    private void OnDisable()
    {
        input.MoveInput -= Move;
    }
    
    public override void Attack()
    {
        var weaponDistance = 2;
        var weaponRadius = 90;
        
        var hitUnits = Physics2D.OverlapCircleAll(transform.position, weaponDistance, attackMask);
        foreach (var unit in hitUnits)
        {
            Vector3 dir = (unit.transform.position - transform.position).normalized;
            var angle = Vector2.Angle(dir, lastMoveDir);
            if (angle < weaponRadius / 2f)
                unit.GetComponent<IDamageable>().TakeDamage(Damage);
        }
    }
}


