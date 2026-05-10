using System.Linq;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public LeaderboardEntryUI entryPrefab;
    public int maxEntries = 5;

    private void OnEnable()
    {
        // usuń stare wpisy
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // dodaj nowe wpisy
        LeaderboardManager.LeaderboardData data = LeaderboardManager.instance.LoadScores();
        var entries = data.entries.OrderByDescending(e => e.survivedTimespan).Take(maxEntries);
        foreach (var entry in entries)
        {
            Instantiate(entryPrefab, transform).Setup(entry);
        }
    }

}
