using System;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public float clearTime;

    public LeaderboardEntry(string playerName, int score, float clearTime)
    {
        this.playerName = playerName;
        this.score = score;
        this.clearTime = clearTime;
    }
    
}