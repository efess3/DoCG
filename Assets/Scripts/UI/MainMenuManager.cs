using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        // Ensure only main menu is visible at start
        ShowMainMenu();
    }

    public void OpenSettings()
    {
        if (SettingsPopupManager.Instance != null)
        {
            SettingsPopupManager.Instance.OpenSettings();
        }
        else if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        }
    }

    public void OpenLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        ShowMainMenu();
    }
}
