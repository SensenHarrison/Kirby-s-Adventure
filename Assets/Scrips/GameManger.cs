using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;
    
    public int remainingEnemies = 10;
    
    [Header("Continue Settings")]
    public Transform player;
    public Transform respawnPoint;
    public PlayerHealth playerHealth;
    public ScoreTimerManager scoreTimerManager;
    public int continuePenalty = 1000;
    
    private bool isPaused = false;
    private bool isGameEnded = false;

    void Start()
    {
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameEnded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }

    public void PauseGame()
    {
        if (isGameEnded) return;

        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (isGameEnded) return;

        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void EnemyDefeated()
    {
        if (isGameEnded) return;

        remainingEnemies--;

        if (remainingEnemies <= 0)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        isGameEnded = true;
        isPaused = false;

        pausePanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(true);

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "GameScene2")
        {
            SaveClearResult();
        }

        Time.timeScale = 0f;
    }
    private void SaveClearResult()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager.Instance is null");
            return;
        }

        if (ScoreTimerManager.Instance == null)
        {
            Debug.LogWarning("ScoreTimerManager.Instance is null");
            return;
        }

        DataManager.Instance.SaveResult(
            ScoreTimerManager.Instance.CurrentScore,
            ScoreTimerManager.Instance.ClearTime,
            true
        );
    }
    
    public void ProceedToNextLevel()
    {
        Time.timeScale = 1f;

        if (ScoreTimerManager.Instance != null)
        {
            ScoreTimerManager.Instance.ChangeScore(2000);
        }

        SceneManager.LoadScene("GameScene2");
    }
    
    public void GameOver()
    {
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void ContinueGame()
    {
        if (player == null || respawnPoint == null || playerHealth == null || ScoreTimerManager.Instance == null)
        {
            return;
        }

        player.gameObject.SetActive(true);
        player.position = respawnPoint.position;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }

        playerHealth.ResetHealth();

        ScoreTimerManager.Instance.ChangeScore(-continuePenalty);

        losePanel.SetActive(false);
        isGameEnded = false;
        isPaused = false;
        Time.timeScale = 1f;
    }
}