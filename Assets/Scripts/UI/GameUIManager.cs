using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;

    void Update()
    {
        if (GameManager.instance == null) return;

        UpdateTimerUI();
        UpdateKillsUI();
    }

    void UpdateTimerUI()
    {
        if (timeText == null) return;

        float time = GameManager.instance.gameTime;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateKillsUI()
    {
        if (killsText == null) return;

        killsText.text = "💀 " + GameManager.instance.killCount.ToString();
    }
}
