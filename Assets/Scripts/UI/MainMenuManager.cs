using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        // Ensure only main menu is visible at start
        ShowMainMenu();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (upgradesPanel != null) upgradesPanel.SetActive(false);
        }
    }

    public void OpenUpgrades()
    {
        if (upgradesPanel != null)
        {
            upgradesPanel.SetActive(true);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (upgradesPanel != null) upgradesPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        ShowMainMenu();
    }
}
