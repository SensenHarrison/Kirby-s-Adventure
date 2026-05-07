using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeadboardText : MonoBehaviour
{
    public Text leaderboardText;
    void OnEnable()
    {
        ShowLeaderboard();
    }
    public void ShowLeaderboard()
    {
        List<LeaderboardEntry> entries = DataManager.Instance.GetLeaderboard();

        if (entries.Count == 0)
        {
            leaderboardText.text += "No records yet.";
            return;
        }

        int displayCount = Mathf.Min(5, entries.Count);

        for (int i = 0; i < displayCount; i++)
        {
            leaderboardText.text +=
                $"{i + 1}. {entries[i].playerName} | Score: {entries[i].score} | Time: {entries[i].clearTime:F2}s\n";
        }
    }
}
