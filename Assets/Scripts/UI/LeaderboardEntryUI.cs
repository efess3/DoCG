using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI levelText;

    public void Setup(LeaderboardManager.LeaderboardEntry entry)
    {
        dateText.text = entry.startDateTime;
        System.TimeSpan time = System.TimeSpan.FromSeconds(entry.survivedTimespan);
        timeText.text = $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
        killsText.text = entry.mobsKilledCount.ToString();
        levelText.text = entry.level.ToString();
    }
}
