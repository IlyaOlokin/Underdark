using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
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
        enemy.OnUnitDeath += StartCountDown;
        timer = respawnDelay;
    }

    private void Update()
    {
        if (enemy.gameObject.activeSelf && Vector2.Distance(enemy.transform.position, player.transform.position) > deactivateRange)
        {
            enemy.gameObject.SetActive(false);
        }
        else if (!enemy.gameObject.activeSelf && Vector2.Distance(enemy.transform.position, player.transform.position) < deactivateRange)
        {
            enemy.gameObject.SetActive(true);
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
        enemy.gameObject.SetActive(true);
        enemy.transform.localPosition = Vector3.zero; 
        isEnemyDead = false;
        timer = respawnDelay;
    }

    private void OnDisable()
    {
        enemy.OnUnitDeath -= StartCountDown;
    }
}
