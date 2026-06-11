using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mapSelectPanel;

    private void Start()
    {
        if (settingsPanel != null)
        {
            GameSettingsManager.InitializeSettingsPanel(settingsPanel);
        }
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
            GameSettingsManager.InitializeSettingsPanel(settingsPanel);
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
        if (mapSelectPanel != null) mapSelectPanel.SetActive(false);
    }

    public void OpenMapSelect()
    {
        if (mapSelectPanel != null) mapSelectPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        ShowMainMenu();
    }
}
