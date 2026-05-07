using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDamagePickup : MonoBehaviour
{
    public int bonusScore = 2000;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
        if (playerShoot != null && playerShoot.playerBullet != null)
        {
            playerShoot.playerBullet.DoubleDamage();
        }

        if (ScoreTimerManager.Instance != null)
        {
            ScoreTimerManager.Instance.ChangeScore(bonusScore);
        }

        Destroy(gameObject);
    }
}