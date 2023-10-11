using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Enemy : Unit
{
    private Player player;
    [SerializeField] private NavMeshAgent agent;
    
    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected override void Update()
    {
        base.Update();
        agent.SetDestination(player.transform.position);
        
        RotateAttackDir();

        if (attackCDTimer < 0 && Vector2.Distance(player.transform.position, transform.position) <= 2)
            Attack();
    }
    
    private void RotateAttackDir()
    {
        var dirToPlayer = player.transform.position - transform.position;
        attackDirAngle = Vector3.Angle(Vector3.right, dirToPlayer);
        if (dirToPlayer.y < 0) attackDirAngle *= -1;
        baseAttackCollider.transform.eulerAngles = new Vector3(0, 0, attackDirAngle - 90);
    }
    
}
