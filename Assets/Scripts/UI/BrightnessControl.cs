using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DoCG.UI
{
    /// <summary>
    /// Manages brightness levels (1 to 10, default 5) using inspector-assigned
    /// up/down buttons and text value label.
    /// </summary>
    public class BrightnessControl : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private TextMeshProUGUI valueLabel;

        [Header("Settings")]
        [SerializeField] private int defaultValue = 5;
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 10;

        public static event Action<float> OnBrightnessChanged;

        private int currentValue;
        private const string Key = "Setting_Brightness";

        private void Awake()
        {
            if (upButton != null) upButton.onClick.AddListener(StepUp);
            if (downButton != null) downButton.onClick.AddListener(StepDown);
        }

        private void Start() => Refresh();

        /// <summary>
        /// Re-reads from PlayerPrefs and fires update.
        /// </summary>
        public void Refresh()
        {
            currentValue = PlayerPrefs.GetInt(Key, defaultValue);
            UpdateLabel();
            
            float multiplier = GameSettingsManager.GetBrightnessMultiplier(currentValue);
            OnBrightnessChanged?.Invoke(multiplier);
        }

        private void StepUp() => Apply(currentValue + 1);
        private void StepDown() => Apply(currentValue - 1);

        private void Apply(int raw)
        {
            currentValue = Mathf.Clamp(raw, minValue, maxValue);

            PlayerPrefs.SetInt(Key, currentValue);
            PlayerPrefs.Save();

            UpdateLabel();

            float multiplier = GameSettingsManager.GetBrightnessMultiplier(currentValue);
            GameSettingsManager.UpdateBrightness(multiplier);
            OnBrightnessChanged?.Invoke(multiplier);
        }

        private void UpdateLabel()
        {
            if (valueLabel != null)
            {
                valueLabel.text = currentValue.ToString();
            }
        }
    }
}
