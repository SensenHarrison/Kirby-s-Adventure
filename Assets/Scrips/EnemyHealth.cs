using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int currentHealth = 4;
    public int maxHealth = 4;
    public GameManger gameManger;
    public EnemySpawnerPlus spawner;

    public void SetSpawner(EnemySpawnerPlus enemySpawner)
    {
        spawner = enemySpawner;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
            
            if (ScoreTimerManager.Instance != null)
            {
                ScoreTimerManager.Instance.ChangeScore(maxHealth * 50);
            }
            
            if (spawner != null)
            {
                spawner.OnEnemyRemoved();
            }

            if (gameManger != null)
            {
                gameManger.EnemyDefeated();
            }
            
            gameObject.SetActive(false);
        }
    }
}
