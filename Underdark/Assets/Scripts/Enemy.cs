using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        agent.SetDestination(player.transform.position);
    }
    
}
