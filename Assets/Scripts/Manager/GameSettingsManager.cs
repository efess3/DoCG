using System;
using UnityEngine;
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

    public static GameSettingsManager Instance { get; private set; }

    // ── Current values (read these from any script) ────────────────────────────
    public static bool AutoAim         { get; private set; } = true;
    public static bool ShowPickupRange { get; private set; } = true;

    // ── Static events fired when a setting changes ─────────────────────────────
    public static event Action<bool> OnAutoAimChanged;
    public static event Action<bool> OnShowPickupRangeChanged;

    // ──────────────────────────────────────────────────────────────────────────

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        if (Instance != null) return;
        var go = new GameObject(nameof(GameSettingsManager));
        Instance = go.AddComponent<GameSettingsManager>();
        DontDestroyOnLoad(go);
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
        AutoAim         = PlayerPrefs.GetInt(AutoAimKey,         1) == 1;
        ShowPickupRange = PlayerPrefs.GetInt(ShowPickupRangeKey, 1) == 1;
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
        }
    }
}
