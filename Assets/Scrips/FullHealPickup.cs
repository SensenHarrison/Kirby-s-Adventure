using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullHealPickup : MonoBehaviour
{
    public int bonusScore = 2000;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreFullHealth();
        }

        if (ScoreTimerManager.Instance != null)
        {
            ScoreTimerManager.Instance.ChangeScore(bonusScore);
        }

        Destroy(gameObject);
    }
}