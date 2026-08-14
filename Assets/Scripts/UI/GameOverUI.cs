using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Game Over UI References")]
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI finalKillsText;
    public TextMeshProUGUI finalLvlText;

    void OnEnable()
    {
        // This runs every time the GameOver panel is activated
        if (GameManager.instance == null) return;

        // Display final time
        if (finalTimeText != null)
        {
            float time = GameManager.instance.gameTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            finalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Display final kills
        if (finalKillsText != null)
        {
            finalKillsText.text = GameManager.instance.killCount.ToString();
        }

        if (finalLvlText != null)
        {
            PlayerLevel playerLevel = FindFirstObjectByType<PlayerLevel>();
            finalLvlText.text = playerLevel != null ? playerLevel.level.ToString() : "1";
        }
    }
}
