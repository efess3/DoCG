using UnityEngine;
using TMPro;

/// <summary>
/// A world-space floating text component that displays damage numbers.
/// Animates in a physical arc (jump and fall due to gravity), shrinks, and fades out.
/// </summary>
public class DamageNumber : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float lifetime = 0.65f;
    private float timer = 0f;

    private Vector3 velocity;
    private float gravity = 14f; // Downward acceleration
    private Color startColor;

    /// <summary>
    /// Initializes the damage text with value, position, and styling.
    /// </summary>
    public void Initialize(float damage, Vector3 position, bool isCritical = false)
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            textMesh = gameObject.AddComponent<TextMeshPro>();
        }

        // Set default font to ensure correct rendering in empty scene loads
        if (TMP_Settings.defaultFontAsset != null)
        {
            textMesh.font = TMP_Settings.defaultFontAsset;
        }

        // Set text value
        textMesh.text = Mathf.RoundToInt(damage).ToString();
        
        // Premium typography sizing
        textMesh.fontSize = isCritical ? 6f : 4.5f;
        textMesh.alignment = TextAlignmentOptions.Center;

        // Make all damage numbers bold/thick for better legibility
        textMesh.fontStyle = FontStyles.Bold;

        // Custom curated colors for Rich Aesthetics
        if (isCritical)
        {
            startColor = new Color(1f, 0.15f, 0.15f); // Vibrant Crimson/Red
        }
        else
        {
            startColor = new Color(1f, 0.85f, 0.2f); // Warm Gold/Yellow
        }
        textMesh.color = startColor;

        // Ensure it displays in front of game elements
        textMesh.sortingOrder = 100;

        // Random offset slightly above the monster's center
        transform.position = position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.6f, 0f);

        // Physics velocity: jump up and arc closer vertically (reduced horizontal speed)
        float vx = UnityEngine.Random.Range(-0.8f, 0.8f);
        float vy = UnityEngine.Random.Range(4.5f, 6.2f);
        velocity = new Vector3(vx, vy, 0f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Apply gravity to velocity and translate position
        velocity.y -= gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Smooth fade out
        float progress = timer / lifetime;
        float alpha = Mathf.Clamp01(1f - progress);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // Punchy shrink effect towards the end
        float scale = Mathf.Lerp(1.2f, 0.8f, progress);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
