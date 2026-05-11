using UnityEngine;

/// <summary>
/// Draws a visible circle around the player showing the exp pickup (magnet) radius.
/// Rendered using a LineRenderer — no extra sprites or prefabs required.
///
/// Visibility is polled every frame from PlayerPrefs (same approach as
/// AutoAimController which polls GameSettingsManager.AutoAim), so it works
/// reliably across scene transitions and regardless of event timing.
///
/// SETUP IN UNITY:
///   1. Add this component to the Player GameObject (same object as PlayerMagnet).
///   2. Create a Material for the circle (e.g. Sprites/Default with a colour of your choice).
///      Assign it to the "Circle Material" field.
///   3. In Settings, bind a SettingsToggle with settingKey = "Setting_ShowPickupRange"
///      to trigger the ON/OFF toggle.
///
/// The component automatically reads the radius from PlayerMagnet, so upgrading
/// the magnet radius via IncreaseMagnetRadius() is reflected immediately.
/// </summary>
[RequireComponent(typeof(PlayerMagnet))]
public class PickupRangeVisualizer : MonoBehaviour
{
    [Header("Visual Style")]
    [Tooltip("Material used for the LineRenderer (Sprites/Default works well).")]
    [SerializeField] private Material circleMaterial;

    [Tooltip("Width of the circle line in world units.")]
    [SerializeField] private float lineWidth = 0.06f;

    [Tooltip("Number of segments — higher = smoother circle.")]
    [SerializeField][Range(32, 128)] private int segments = 64;

    [Tooltip("Colour of the pickup range circle.")]
    [SerializeField] private Color circleColor = new Color(0.4f, 0.9f, 1f, 0.55f);

    // ── Internal ──────────────────────────────────────────────────────────────
    private LineRenderer lineRenderer;
    private PlayerMagnet magnet;
    private float lastRadius = -1f;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        magnet = GetComponent<PlayerMagnet>();

        // Use existing LineRenderer if present, otherwise create one
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            BuildLineRenderer();
        else
            ConfigureLineRenderer();
    }

    private void Start()
    {
        // All Awake() calls are done by now — read setting and draw
        bool show = PlayerPrefs.GetInt(GameSettingsManager.ShowPickupRangeKey, 1) == 1;
        lineRenderer.enabled = show;
        DrawCircle(magnet.magnetRadius);
    }

    /// <summary>
    /// Every frame: sync visibility with the setting AND update radius if needed.
    /// Polling is cheap (one PlayerPrefs.GetInt + one float compare) and bulletproof.
    /// Same approach as AutoAimController.Update() which polls GameSettingsManager.AutoAim.
    /// </summary>
    private void LateUpdate()
    {
        if (lineRenderer == null) return;

        // ── Sync visibility with setting ─────────────────────────────────────
        bool shouldShow = PlayerPrefs.GetInt(GameSettingsManager.ShowPickupRangeKey, 1) == 1;
        if (lineRenderer.enabled != shouldShow)
            lineRenderer.enabled = shouldShow;

        if (!lineRenderer.enabled) return;

        // ── Update circle radius if it changed (e.g. after magnet upgrade) ───
        float r = magnet.magnetRadius;
        if (!Mathf.Approximately(r, lastRadius))
            DrawCircle(r);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void BuildLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        ConfigureLineRenderer();
    }

    private void ConfigureLineRenderer()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if (circleMaterial != null)
            lineRenderer.material = circleMaterial;

        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;

        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 5;
    }

    private void DrawCircle(float radius)
    {
        lastRadius = radius;

        float angleStep = 360f / segments;

        // Kompensujemy skalę gracza. LineRenderer w trybie local space dziedziczy skalę z Transform.
        // Dzieląc przez skalę gwarantujemy, że ostateczny zasięg w świecie (world space)
        // będzie idealnie równy zmiennej 'radius', z którą działa fizyka (OverlapCircleAll).
        float scaleX = Mathf.Abs(transform.lossyScale.x) > 0.001f ? Mathf.Abs(transform.lossyScale.x) : 1f;
        float scaleY = Mathf.Abs(transform.lossyScale.y) > 0.001f ? Mathf.Abs(transform.lossyScale.y) : 1f;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = (Mathf.Cos(angle) * radius) / scaleX;
            float y = (Mathf.Sin(angle) * radius) / scaleY;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
