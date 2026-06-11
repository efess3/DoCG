using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A persistent manager that generates a full-screen canvas overlay with an Image component.
/// Controls the alpha/color of the image to implement a universal brightness system.
/// </summary>
public class BrightnessOverlayManager : MonoBehaviour
{
    public static BrightnessOverlayManager Instance { get; private set; }

    private Canvas overlayCanvas;
    private Image overlayImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateOverlayCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DoCG.UI.BrightnessControl.OnBrightnessChanged += UpdateBrightness;
        // Load initial value from GameSettingsManager
        UpdateBrightness(GameSettingsManager.Brightness);
    }

    private void OnDestroy()
    {
        DoCG.UI.BrightnessControl.OnBrightnessChanged -= UpdateBrightness;
    }

    private void CreateOverlayCanvas()
    {
        // Create Canvas GameObject
        GameObject canvasGo = new GameObject("BrightnessOverlayCanvas");
        DontDestroyOnLoad(canvasGo);

        overlayCanvas = canvasGo.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 99999; // Above all UI

        canvasGo.AddComponent<CanvasScaler>();

        // Add full-screen image
        GameObject imageGo = new GameObject("OverlayImage");
        imageGo.transform.SetParent(canvasGo.transform, false);

        overlayImage = imageGo.AddComponent<Image>();
        overlayImage.color = Color.clear;
        overlayImage.raycastTarget = false; // CRITICAL: Allow clicks to pass through!

        // Stretch image to fill screen
        RectTransform rect = overlayImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public void UpdateBrightness(float brightnessValue)
    {
        if (overlayImage == null) return;

        if (Mathf.Approximately(brightnessValue, 1.0f))
        {
            overlayImage.color = Color.clear;
            overlayImage.gameObject.SetActive(false);
        }
        else if (brightnessValue < 1.0f)
        {
            // Darken screen (Black overlay with alpha)
            overlayImage.gameObject.SetActive(true);
            float alpha = (1.0f - brightnessValue) * 0.8f; // Max opacity 80% at min value 0.5f (gives 0.4 opacity)
            overlayImage.color = new Color(0f, 0f, 0f, alpha);
        }
        else
        {
            // Brighten screen (White overlay with alpha)
            overlayImage.gameObject.SetActive(true);
            float alpha = (brightnessValue - 1.0f) * 0.5f; // Max opacity 25% at max value 1.5f (gives 0.25 opacity)
            overlayImage.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
