using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class EnemySpawnPoint : MonoBehaviour
{
    [FormerlySerializedAs("enemy")] [SerializeField] private NPCUnit npcUnit;
    [SerializeField] private float respawnDelay;
    [SerializeField] private float deactivateRange = 25;
    private float timer;
    private bool isEnemyDead;
    
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }
    
    private void Awake()
    {
        npcUnit.OnUnitDeath += StartCountDown;
        timer = respawnDelay;
    }

    private void Update()
    {
        if (npcUnit.gameObject.activeSelf && Vector2.Distance(npcUnit.transform.position, player.transform.position) > deactivateRange)
        {
            npcUnit.gameObject.SetActive(false);
        }
        else if (!npcUnit.gameObject.activeSelf && !isEnemyDead && Vector2.Distance(npcUnit.transform.position, player.transform.position) < deactivateRange)
        {
            npcUnit.gameObject.SetActive(true);
        }
        
        if (!isEnemyDead) return;
        
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            RespawnEnemy();
        }
    }

    private void StartCountDown()
    {
        isEnemyDead = true;
    }

    private void RespawnEnemy()
    {
        npcUnit.gameObject.SetActive(true);
        npcUnit.transform.localPosition = Vector3.zero; 
        isEnemyDead = false;
        timer = respawnDelay;
    }

    private void OnDisable()
    {
        npcUnit.OnUnitDeath -= StartCountDown;
    }
}
