using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("Czas w sekundach do wybuchu")]
    public float delayBeforeExplosion = 3f;
    [Tooltip("Prefab efektu wybuchu")]
    public GameObject explosionEffectPrefab;

    [Header("Pulse Effect")]
    [Tooltip("Referencja do SpriteRenderer, który będzie pulsował")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Kolor do którego dąży puls (np. czerwony ostrzegawczy)")]
    public Color pulseColor = Color.red;

    private Color originalColor;
    private float timer;
    private float pulsePhase;
    private bool hasExploded = false;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        if (hasExploded) return;

        timer += Time.deltaTime;
        float progress = timer / delayBeforeExplosion;

        // Efekt pulsowania (analogiczny do EnemySpliter)
        if (spriteRenderer != null)
        {
            // Przyspieszanie częstotliwości pulsu wraz ze zbliżaniem się do wybuchu
            float currentFrequency = Mathf.Lerp(5f, 40f, progress);
            pulsePhase += currentFrequency * Time.deltaTime;

            // Wartość od 0 do 1
            float pulse = (Mathf.Sin(pulsePhase) + 1f) / 2f;

            spriteRenderer.color = Color.Lerp(originalColor, pulseColor, pulse);
        }

        if (timer >= delayBeforeExplosion)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Usunięcie bomby
        Destroy(gameObject);
    }
}
