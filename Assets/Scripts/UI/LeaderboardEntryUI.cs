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
        timeText.text = entry.survivedTimespan.ToString("F2");
        killsText.text = entry.mobsKilledCount.ToString();
        levelText.text = entry.level.ToString();
    }
}
