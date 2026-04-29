using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsPopupManager : MonoBehaviour
{
    public static SettingsPopupManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("State")]
    private bool isPaused = false;
    private bool isMenuScene = false;

    private void Awake()
    {
        Instance = this;
        UpdateSceneState();
    }

    private void UpdateSceneState()
    {
        isMenuScene = SceneManager.GetActiveScene().name == "MenuScene";
    }

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        if (settingsPanel == null) return;

        bool newState = !settingsPanel.activeSelf;
        settingsPanel.SetActive(newState);

        // Handle pausing only in game scene
        if (!isMenuScene)
        {
            if (newState)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(true);
        if (!isMenuScene) PauseGame();
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(false);
        if (!isMenuScene) ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        // Optional: Lock/Unlock cursor if needed
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Only lock cursor if in game
        if (!isMenuScene)
        {
            // Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Call this from a 'Back to Menu' button in the settings popup
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}
