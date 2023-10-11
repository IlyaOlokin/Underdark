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

    [SerializeField] private PolygonCollider2D baseAttackCollider;
    [SerializeField] private LayerMask attackMask;
    private float attackDirAngle;
    public event Action<float, float, float> OnBaseAttack;
    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SetAttackCollider(90);
        
        attackDirAngle = Vector3.Angle(Vector3.right, lastMoveDir);
        if (lastMoveDir.y < 0) attackDirAngle *= -1;

        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }

    private void SetAttackCollider(float radius)
    {
        int pointStep = 10;
        int pointsCount = (int) radius / pointStep + 1;
        List<Vector2> path = new List<Vector2>();
        float currentPointAngle = -radius / 2f;
        for (int i = 0; i < pointsCount; i++)
        {
            var sin = Mathf.Sin(Mathf.Deg2Rad * currentPointAngle) * 2; // * attackDistance
            var cos = Mathf.Cos(Mathf.Deg2Rad * currentPointAngle) * 2; // * attackDistance
            path.Add(new Vector2(sin, cos));
            currentPointAngle += pointStep;
        }
        path.Add(baseAttackCollider.transform.localPosition);
        baseAttackCollider.SetPath(0, path);
    }
    
    public override void Attack()
    {
        var weaponDistance = 2;
        var weaponRadius = 90;
        
        var contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(attackMask);
        List<Collider2D> hitUnits = new List<Collider2D>();
        baseAttackCollider.OverlapCollider(contactFilter, hitUnits);
        
        foreach (var unit in hitUnits)
        {
            unit.GetComponent<IDamageable>().TakeDamage(Damage);
        }
        
        OnBaseAttack?.Invoke(attackDirAngle, weaponRadius, weaponDistance);
    }
}


