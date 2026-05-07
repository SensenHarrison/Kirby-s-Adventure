using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreTimerManager : MonoBehaviour
{
    public static ScoreTimerManager Instance;

    public Text timeText;
    public Text scoreText;

    public int startScore = 2000;

    private int score;
    private int elapsedSeconds;
    private float timer;

    public int CurrentScore => score;
    public float ClearTime => elapsedSeconds;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
            return;
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (Instance == this)
            {
                Instance = null;
            }

            Destroy(gameObject);
            return;
        }

        GameObject timeObj = GameObject.FindWithTag("TimeText");
        GameObject scoreObj = GameObject.FindWithTag("ScoreText");

        if (timeObj != null)
        {
            timeText = timeObj.GetComponent<Text>();
        }

        if (scoreObj != null)
        {
            scoreText = scoreObj.GetComponent<Text>();
        }

        UpdateUI();
    }

    void Start()
    {
        score = startScore;
        elapsedSeconds = 0;
        timer = 0f;
        UpdateUI();
    }

    void Update()
    {
        timer += Time.deltaTime;

        while (timer >= 1f)
        {
            timer -= 1f;
            elapsedSeconds++;
            score = Mathf.Max(0, score - 1);
            UpdateUI();
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        score = Mathf.Max(0, score);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText == null || scoreText == null) return;

        int minutes = elapsedSeconds / 60;
        int seconds = elapsedSeconds % 60;

        timeText.text = $"Time:{minutes:00}'{seconds:00}\"";
        scoreText.text = "Score: " + score;
    }
}