using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsPopupManager : MonoBehaviour
{
    private const string PauseStatsPanelName = "PauseStatsPanel";
    private const string PauseStatsTextName = "PauseStatsText";

    public static SettingsPopupManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("State")]
    private bool isPaused = false;
    private bool isMenuScene = false;
    private TextMeshProUGUI pauseStatsText;

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

        if (!isMenuScene)
            EnsurePauseStatsUI();
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
                RefreshPauseStats();
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
        
        // Force all toggles to re-read from PlayerPrefs
        // This is critical for cross-scene sync
        RefreshAllSettingsUI();

        if (!isMenuScene)
        {
            RefreshPauseStats();
            PauseGame();
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(false);
        if (!isMenuScene) ResumeGame();
    }

    private void RefreshAllSettingsUI()
    {
        if (settingsPanel == null) return;

        var toggles = settingsPanel.GetComponentsInChildren<DoCG.UI.SettingsToggle>(true);
        foreach (var toggle in toggles)
            toggle.Refresh();

        var volumes = settingsPanel.GetComponentsInChildren<DoCG.UI.VolumeControl>(true);
        foreach (var vol in volumes)
            vol.Refresh();
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

    private void RefreshPauseStats()
    {
        if (isMenuScene || settingsPanel == null)
            return;

        TextMeshProUGUI statsLabel = EnsurePauseStatsUI();
        if (statsLabel == null)
            return;

        if (UpgradeManager.instance == null)
        {
            statsLabel.text = "PLAYER STATS\nUpgradeManager not found.";
            return;
        }

        statsLabel.text = UpgradeManager.instance.GetPauseStatsText();
    }

    private TextMeshProUGUI EnsurePauseStatsUI()
    {
        if (pauseStatsText != null)
            return pauseStatsText;

        if (settingsPanel == null)
            return null;

        Transform statsRoot = settingsPanel.transform;
        Transform existingPanel = FindPauseStatsPanel(statsRoot);
        if (existingPanel != null)
        {
            if (existingPanel.parent != statsRoot)
                existingPanel.SetParent(statsRoot, false);

            ConfigurePauseStatsPanelRect(existingPanel.GetComponent<RectTransform>());
            pauseStatsText = existingPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            if (pauseStatsText != null)
            {
                ConfigurePauseStatsTextRect(pauseStatsText.rectTransform);
                ApplyPauseStatsTextStyle(pauseStatsText);
                return pauseStatsText;
            }
        }

        GameObject statsPanelObject = new GameObject(PauseStatsPanelName, typeof(RectTransform), typeof(Image));
        statsPanelObject.transform.SetParent(statsRoot, false);

        RectTransform statsPanelRect = statsPanelObject.GetComponent<RectTransform>();
        ConfigurePauseStatsPanelRect(statsPanelRect);

        Image background = statsPanelObject.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.45f);
        background.raycastTarget = false;

        GameObject statsTextObject = new GameObject(PauseStatsTextName, typeof(RectTransform), typeof(TextMeshProUGUI));
        statsTextObject.transform.SetParent(statsPanelObject.transform, false);

        RectTransform statsTextRect = statsTextObject.GetComponent<RectTransform>();
        ConfigurePauseStatsTextRect(statsTextRect);

        pauseStatsText = statsTextObject.GetComponent<TextMeshProUGUI>();
        ApplyPauseStatsTextStyle(pauseStatsText);

        return pauseStatsText;
    }

    private static void ConfigurePauseStatsPanelRect(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return;

        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.anchorMax = new Vector2(0f, 0.5f);
        rectTransform.pivot = new Vector2(0f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(24f, 0f);
        rectTransform.sizeDelta = new Vector2(360f, 760f);
    }

    private static void ConfigurePauseStatsTextRect(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(18f, 18f);
        rectTransform.offsetMax = new Vector2(-18f, -18f);
    }

    private void ApplyPauseStatsTextStyle(TextMeshProUGUI targetText)
    {
        if (targetText == null)
            return;

        TextMeshProUGUI referenceText = FindPauseStatsReferenceText();
        if (referenceText != null)
        {
            targetText.font = referenceText.font;
            targetText.fontSharedMaterial = referenceText.fontSharedMaterial;
            targetText.fontStyle = referenceText.fontStyle;
            targetText.characterSpacing = referenceText.characterSpacing;
            targetText.lineSpacing = referenceText.lineSpacing;
        }
        else
        {
            targetText.font = TMP_Settings.defaultFontAsset;
        }

        targetText.fontSize = 21f;
        targetText.alignment = TextAlignmentOptions.TopLeft;
        targetText.enableWordWrapping = false;
        targetText.overflowMode = TextOverflowModes.Overflow;
        targetText.color = Color.white;
        targetText.raycastTarget = false;
    }

    private TextMeshProUGUI FindPauseStatsReferenceText()
    {
        if (settingsPanel == null)
            return null;

        TextMeshProUGUI[] textComponents = settingsPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            if (textComponent != null && textComponent != pauseStatsText)
                return textComponent;
        }

        return null;
    }

    private Transform FindPauseStatsPanel(Transform root)
    {
        if (root == null)
            return null;

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == PauseStatsPanelName)
                return child;
        }

        return null;
    }
}
