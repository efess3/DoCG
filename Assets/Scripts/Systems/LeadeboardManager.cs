using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class LeaderboardManager : MonoBehaviour
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string startDateTime;
        public float survivedTimespan;
        public int level;
        public int mobsKilledCount;

        public LeaderboardEntry(string startDateTime, float survivedTimespan, int level, int mobsKilledCount)
        {
            this.startDateTime = startDateTime;
            this.survivedTimespan = survivedTimespan;
            this.level = level;
            this.mobsKilledCount = mobsKilledCount;
        }
    }

    [Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }
    public static LeaderboardManager instance;
    private string filePath;

    private void Awake()
    {
        instance = this;

        Directory.CreateDirectory(Application.persistentDataPath);

        filePath = Path.Combine(
            Application.persistentDataPath,
            "leaderboard.json"
        );
    }

    public void AddEntry(int level, int kills, float duration, string startTime)
    {
        LeaderboardData data = LoadScores();

        LeaderboardEntry newEntry = new LeaderboardEntry(startTime, duration, level, kills);
        data.entries.Add(newEntry);

        // Opcjonalnie: sortowanie po czasie przeżycia (malejąco)
        data.entries.Sort((a, b) => b.survivedTimespan.CompareTo(a.survivedTimespan));

        SaveScores(data);
        Debug.Log("Leaderboard entry added and saved to: " + filePath);
    }

    public LeaderboardData LoadScores()
    {
        Directory.CreateDirectory(Application.persistentDataPath);

        if (!File.Exists(filePath))
            return new LeaderboardData();

        string json = File.ReadAllText(filePath);
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

        return data ?? new LeaderboardData();
    }
    private void SaveScores(LeaderboardData data)
    {
        Directory.CreateDirectory(Application.persistentDataPath);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }
}
