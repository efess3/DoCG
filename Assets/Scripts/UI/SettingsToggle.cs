using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace DoCG.UI
{
    public class SettingsToggle : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button onButton;
        [SerializeField] private Button offButton;

        [Header("Sprites")]
        [SerializeField] private Sprite onActiveSprite;   // Green
        [SerializeField] private Sprite onInactiveSprite; // Grey
        [SerializeField] private Sprite offActiveSprite;  // Red
        [SerializeField] private Sprite offInactiveSprite;// Grey

        [Header("Setting Key")]
        [SerializeField] private string settingKey = "Setting_Name";
        [SerializeField] private bool defaultValue = true;

        [Header("Events")]
        public UnityEvent<bool> OnSettingChanged;

        private bool currentState;

        private void Awake()
        {
            // Auto-fill if attached to one of the buttons
            if (onButton == null) onButton = GetComponent<Button>();
        }

        private void Start()
        {
            // Load state from PlayerPrefs (1 = true, 0 = false)
            currentState = PlayerPrefs.GetInt(settingKey, defaultValue ? 1 : 0) == 1;
            
            // Add listeners to buttons
            if (onButton != null)
                onButton.onClick.AddListener(() => SetState(true));
            
            if (offButton != null)
                offButton.onClick.AddListener(() => SetState(false));

            // Initial visual update
            UpdateVisuals();
            
            // Initial event trigger to sync other systems
            OnSettingChanged?.Invoke(currentState);
        }

        /// <summary>
        /// Updates the state of the setting and saves it.
        /// </summary>
        /// <param name="isOn">True for ON, False for OFF</param>
        public void SetState(bool isOn)
        {
            // Avoid redundant updates
            if (currentState == isOn && PlayerPrefs.HasKey(settingKey)) return;

            currentState = isOn;
            PlayerPrefs.SetInt(settingKey, isOn ? 1 : 0);
            PlayerPrefs.Save();
            
            UpdateVisuals();
            
            // Notify listeners
            OnSettingChanged?.Invoke(isOn);
            
            Debug.Log($"Setting [{settingKey}] changed to: {(isOn ? "ON" : "OFF")}");
        }

        private void UpdateVisuals()
        {
            // Logic: 
            // If ON: ON Button = Green, OFF Button = Grey
            // If OFF: ON Button = Grey, OFF Button = Red
            
            if (onButton != null && onButton.image != null)
            {
                onButton.image.sprite = currentState ? onActiveSprite : onInactiveSprite;
            }

            if (offButton != null && offButton.image != null)
            {
                // OFF is "active" (Red) when currentState is false
                offButton.image.sprite = !currentState ? offActiveSprite : offInactiveSprite;
            }
        }
        
        // Helper to get current state from other scripts
        public bool IsOn() => currentState;
    }
}
