using UnityEngine;
using DoCG.UI;

/// <summary>
/// Attach to the Player GameObject.
///
/// Auto Aim ON  → AutoShooter picks nearest enemy automatically. Default cursor.
/// Auto Aim OFF → Player aims with the mouse. Custom crosshair cursor is shown.
///
/// SETUP IN UNITY:
///   1. Add this component to the Player GameObject (same object as AutoShooter).
///   2. Assign a Texture2D as the crosshair cursor in the Inspector.
///      Import settings: Texture Type = Cursor, Read/Write = ON, no compression.
///   3. Hotspot is auto-centered; override HotspotOffset only if your cursor art
///      has the active point somewhere other than the centre.
/// </summary>
public class AutoAimController : MonoBehaviour
{
    [Header("Crosshair Cursor")]
    [Tooltip("Texture2D to use as the cursor in-game. Import as Texture Type = Cursor.")]
    [SerializeField] private Texture2D crosshairTexture;

    [Tooltip("Pixel offset for the active point of the cursor. Leave at (0,0) to auto-center.")]
    [SerializeField] private Vector2 hotspotOffset = Vector2.zero;
    [Tooltip("If true, hotspot is automatically set to the centre of the crosshair texture.")]
    [SerializeField] private bool autoCenterHotspot = true;

    // Reference filled automatically at runtime
    private AutoShooter autoShooter;
    private Camera mainCamera;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        autoShooter = GetComponent<AutoShooter>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Apply the saved setting immediately on scene load
        Apply(GameSettingsManager.AutoAim);
    }

    private void OnEnable()
    {
        SettingsToggle.OnAnySettingChanged += OnSettingChanged;
    }

    private void OnDisable()
    {
        SettingsToggle.OnAnySettingChanged -= OnSettingChanged;
        RestoreDefaultCursor();
    }

    private void OnSettingChanged(string key, bool value)
    {
        if (key == GameSettingsManager.AutoAimKey)
            Apply(value);
    }

    // ── Update (only active when manual aim is ON, i.e. AutoAim setting is OFF) ──

    private void Update()
    {
        // Manual aim is active when AutoAim setting is OFF
        if (GameSettingsManager.AutoAim) return;

        // Pass mouse world position to AutoShooter every frame
        if (mainCamera != null && autoShooter != null)
        {
            // Fix Z: use the camera's nearClipPlane so ScreenToWorldPoint works for 2D
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = mainCamera.nearClipPlane;
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(screenPos);
            autoShooter.SetManualAimTarget(mouseWorld);
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void Apply(bool isAutoAim)
    {
        // AutoAim ON  → nearest-enemy mode (UseManualAim = false), default cursor
        // AutoAim OFF → mouse-aim mode (UseManualAim = true), crosshair cursor
        bool manualAim = !isAutoAim;

        if (autoShooter != null)
            autoShooter.UseManualAim = manualAim;

        if (manualAim)
            SetCustomCursor();
        else
            RestoreDefaultCursor();
    }

    private void SetCustomCursor()
    {
        if (crosshairTexture == null) return;

        Vector2 hotspot = autoCenterHotspot
            ? new Vector2(crosshairTexture.width * 0.5f, crosshairTexture.height * 0.5f)
            : hotspotOffset;

        Cursor.SetCursor(crosshairTexture, hotspot, CursorMode.Auto);
    }

    private void RestoreDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
