using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private string filePath;
    private LeaderboardData leaderboardData = new LeaderboardData();

    private void Awake()
    {
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

        filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    public void SaveResult(int score, float clearTime, bool isWin)
    {
        if (!isWin)
        {
            Debug.Log("Game not won. Result will not be saved.");
            return;
        }

        string playerName = "Player" + (leaderboardData.entries.Count + 1);

        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, clearTime);
        leaderboardData.entries.Add(newEntry);

        leaderboardData.entries.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score);
            if (scoreCompare == 0)
            {
                return a.clearTime.CompareTo(b.clearTime);
            }
            return scoreCompare;
        });

        SaveLeaderboardToFile();
    }

    private void SaveLeaderboardToFile()
    {
        string json = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Leaderboard saved to: " + filePath);
    }

    public void LoadLeaderboard()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);

            if (leaderboardData == null || leaderboardData.entries == null)
            {
                leaderboardData = new LeaderboardData();
            }
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }

    public List<LeaderboardEntry> GetLeaderboard()
    {
        return leaderboardData.entries;
    }

    public void ClearLeaderboard()
    {
        leaderboardData = new LeaderboardData();
        SaveLeaderboardToFile();
    }
}