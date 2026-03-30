using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI killsText;

    void Start()
    {
        if (timeText == null)
        {
            GameObject tObj = GameObject.Find("TimeText");
            if (tObj != null) timeText = tObj.GetComponent<TextMeshProUGUI>();
        }

        if (killsText == null)
        {
            GameObject kObj = GameObject.Find("KillsText");
            if (kObj != null) killsText = kObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        UpdateTimerUI();
        UpdateKillsUI();
    }

    void UpdateTimerUI()
    {
        if (timeText != null)
        {
            float time = GameManager.instance.gameTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateKillsUI()
    {
        if (killsText != null)
        {
            killsText.text = "💀 " + GameManager.instance.killCount.ToString();
        }
    }
}
