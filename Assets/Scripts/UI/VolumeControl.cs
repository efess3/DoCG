using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DoCG.UI
{
    /// <summary>
    /// Attach to an empty parent GameObject that groups a volume row
    /// (UpButton + DownButton + optional value label).
    ///
    /// Hierarchy example:
    ///   MasterVolumeControl  [VolumeControl script here]
    ///   ├── MasterVolumeDown  [Button]
    ///   ├── MasterVolumeUp    [Button]
    ///   └── MasterVolumeText  [TextMeshProUGUI — optional]
    /// </summary>
    public class VolumeControl : MonoBehaviour
    {
        public enum VolumeChannel { Master, Music, SFX }

        [Header("Channel")]
        [SerializeField] private VolumeChannel channel = VolumeChannel.Master;

        [Header("UI References")]
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private TextMeshProUGUI valueLabel; // optional — shows "70%"

        [Header("Settings")]
        [SerializeField][Range(0.05f, 0.5f)] private float step = 0.1f;
        [SerializeField] private float defaultValue = 1f;

        // ---------------------------------------------------------------
        // Static event — subscribe in code (like BackgroundMusicManager)
        // to react to volume changes without Inspector drag & drop.
        // Parameters: (channel, newValue 0.0 – 1.0)
        // ---------------------------------------------------------------
        public static event Action<VolumeChannel, float> OnVolumeChanged;

        private float currentValue;
        private string Key => $"Volume_{channel}";

        // ---------------------------------------------------------------
        private void Awake()
        {
            if (upButton   != null) upButton.onClick.AddListener(StepUp);
            if (downButton != null) downButton.onClick.AddListener(StepDown);
        }

        private void Start() => Refresh();

        /// <summary>
        /// Re-reads from PlayerPrefs and refreshes the label.
        /// Called by SettingsPopupManager.RefreshAllSettingsUI via GetComponentsInChildren.
        /// </summary>
        public void Refresh()
        {
            currentValue = PlayerPrefs.GetFloat(Key, defaultValue);
            UpdateLabel();
            // Notify so audio managers apply the saved value on panel open
            OnVolumeChanged?.Invoke(channel, currentValue);
        }

        // ---------------------------------------------------------------
        private void StepUp()   => Apply(currentValue + step);
        private void StepDown() => Apply(currentValue - step);

        private void Apply(float raw)
        {
            // Round to avoid floating-point drift (0.30000001 etc.)
            float rounded = Mathf.Round(raw / step) * step;
            currentValue  = Mathf.Clamp(rounded, 0f, 1f);

            PlayerPrefs.SetFloat(Key, currentValue);
            PlayerPrefs.Save();

            UpdateLabel();
            OnVolumeChanged?.Invoke(channel, currentValue);
        }

        private void UpdateLabel()
        {
            if (valueLabel != null)
                valueLabel.text = Mathf.RoundToInt(currentValue * 100) + "%";
        }
    }
}
