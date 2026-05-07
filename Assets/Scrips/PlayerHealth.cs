using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth = 10;
    public int maxHealth = 10;

    public Text healthText;
    public Animator healthTextAnim;

    public GameManger gameManager;

    private bool isDead = false;

    private void Start()
    {
        UpdateHealthUI();
    }

    public void ChangeHealth(int amount)
    {
        if (isDead) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextUpdate");
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + "/" + maxHealth;
        }
    }
    
    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;

        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextUpdate");
        }

        UpdateHealthUI();
    }

    private void Die()
    {
        isDead = true;

        if (gameManager != null)
        {
            gameManager.GameOver();
        }

        gameObject.SetActive(false);
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        gameObject.SetActive(true);
        UpdateHealthUI();
    }
}