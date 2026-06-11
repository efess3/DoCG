using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DoCG.UI;

/// <summary>
/// Singleton that tracks boolean game-play settings persisted via PlayerPrefs.
/// Subscribes to SettingsToggle.OnAnySettingChanged so every SettingsToggle panel
/// (menu AND in-game) keeps this in sync automatically.
/// </summary>
public class GameSettingsManager : MonoBehaviour
{
    // ── PlayerPrefs keys (must match settingKey on the SettingsToggle components) ──
    public const string AutoAimKey        = "Setting_AutoAim";
    public const string ShowPickupRangeKey = "Setting_ShowPickupRange";
    public const string ScreenShakeKey       = "Setting_ScreenShake";
    public const string FullScreenKey        = "Setting_FullScreen";
    public const string BrightnessKey        = "Setting_Brightness";
    public const string ShowDamageNumbersKey = "Setting_ShowDamageNumbers";

    public static GameSettingsManager Instance { get; private set; }

    // ── Current values (read these from any script) ────────────────────────────
    public static bool AutoAim         { get; private set; } = true;
    public static bool ShowPickupRange { get; private set; } = true;
    public static bool ScreenShake         { get; private set; } = true;
    public static bool FullScreen          { get; private set; } = true;
    public static float Brightness         { get; private set; } = 1.0f;
    public static bool ShowDamageNumbers   { get; private set; } = true;

    // ── Static events fired when a setting changes ─────────────────────────────
    public static event Action<bool> OnAutoAimChanged;
    public static event Action<bool> OnShowPickupRangeChanged;
    public static event Action<bool> OnScreenShakeChanged;
    public static event Action<bool> OnFullScreenChanged;
    public static event Action<float> OnBrightnessChanged;
    public static event Action<bool> OnShowDamageNumbersChanged;

    // ──────────────────────────────────────────────────────────────────────────

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (Instance != null) return;
        var go = new GameObject(nameof(GameSettingsManager));
        Instance = go.AddComponent<GameSettingsManager>();
        DontDestroyOnLoad(go);

        // Also create BrightnessOverlayManager
        var brightnessGo = new GameObject(nameof(BrightnessOverlayManager));
        brightnessGo.AddComponent<BrightnessOverlayManager>();
        DontDestroyOnLoad(brightnessGo);

        // Also create DamageNumberManager
        var dnmGo = new GameObject(nameof(DamageNumberManager));
        dnmGo.AddComponent<DamageNumberManager>();
        DontDestroyOnLoad(dnmGo);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAll();
        SettingsToggle.OnAnySettingChanged += OnAnySettingChanged;
    }

    private void OnDestroy()
    {
        SettingsToggle.OnAnySettingChanged -= OnAnySettingChanged;
    }

    // ── Reads all values from PlayerPrefs ─────────────────────────────────────
    private void LoadAll()
    {
        AutoAim           = PlayerPrefs.GetInt(AutoAimKey,         1) == 1;
        ShowPickupRange   = PlayerPrefs.GetInt(ShowPickupRangeKey, 1) == 1;
        ScreenShake       = PlayerPrefs.GetInt(ScreenShakeKey,       1) == 1;
        FullScreen        = PlayerPrefs.GetInt(FullScreenKey,        1) == 1;
        ShowDamageNumbers = PlayerPrefs.GetInt(ShowDamageNumbersKey, 1) == 1;
        
        int brightnessLevel = PlayerPrefs.GetInt(BrightnessKey, 5);
        Brightness        = GetBrightnessMultiplier(brightnessLevel);

        // Apply fullscreen immediately
        Screen.fullScreen = FullScreen;
    }

    /// <summary>
    /// Maps brightness level (1-10) to overlay alpha multiplier.
    /// Level 5 is default (1.0f). Levels < 5 darken. Levels > 5 wash out/brighten.
    /// </summary>
    public static float GetBrightnessMultiplier(int level)
    {
        if (level == 5) return 1.0f;
        if (level < 5)
        {
            return 0.5f + (level - 1) * 0.125f; // Map 1-5 to 0.5 - 1.0
        }
        else
        {
            return 1.0f + (level - 5) * 0.1f; // Map 5-10 to 1.0 - 1.5
        }
    }

    // ── Update brightness value from other scripts ──────────────────────────
    public static void UpdateBrightness(float value)
    {
        Brightness = value;
        OnBrightnessChanged?.Invoke(value);
    }

    // ── Receives every SettingsToggle change ──────────────────────────────────
    private void OnAnySettingChanged(string key, bool value)
    {
        switch (key)
        {
            case AutoAimKey:
                AutoAim = value;
                OnAutoAimChanged?.Invoke(value);
                break;

            case ShowPickupRangeKey:
                ShowPickupRange = value;
                OnShowPickupRangeChanged?.Invoke(value);
                break;

            case ScreenShakeKey:
                ScreenShake = value;
                OnScreenShakeChanged?.Invoke(value);
                break;

            case FullScreenKey:
                FullScreen = value;
                Screen.fullScreen = value;
                OnFullScreenChanged?.Invoke(value);
                break;

            case ShowDamageNumbersKey:
                ShowDamageNumbers = value;
                OnShowDamageNumbersChanged?.Invoke(value);
                break;
        }
    }

    /// <summary>
    /// Static helper to dynamically setup settings panel elements at runtime.
    /// Used by both SettingsPopupManager (in-game) and MainMenuManager (main menu)
    /// to ensure the same settings logic and appearance is shared globally.
    /// </summary>
    public static void InitializeSettingsPanel(GameObject settingsPanel)
    {
        if (settingsPanel == null) return;

        // 1. Setup toggle keys
        var toggles = settingsPanel.GetComponentsInChildren<SettingsToggle>(true);
        foreach (var toggle in toggles)
        {
            if (toggle.gameObject.name == "ScreenShakeOn")
            {
                toggle.SetKeyAndDefault(ScreenShakeKey, true);
            }
            else if (toggle.gameObject.name == "FullscreenOn")
            {
                toggle.SetKeyAndDefault(FullScreenKey, true);
            }
            else if (toggle.gameObject.name == "DamageNumbersOn")
            {
                toggle.SetKeyAndDefault(ShowDamageNumbersKey, true);
            }
        }

        // 2. Force all components to refresh
        foreach (var toggle in toggles)
            toggle.Refresh();

        var volumes = settingsPanel.GetComponentsInChildren<VolumeControl>(true);
        foreach (var vol in volumes)
            vol.Refresh();

        var brightnessControls = settingsPanel.GetComponentsInChildren<BrightnessControl>(true);
        foreach (var bc in brightnessControls)
            bc.Refresh();
    }
}

