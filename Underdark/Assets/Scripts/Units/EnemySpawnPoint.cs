using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private float respawnDelay;
    private float timer;
    private bool isEnemyDead;

    private void Awake()
    {
        enemy.OnUnitDeath += StartCountDown;
        timer = respawnDelay;
    }

    private void Update()
    {
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
