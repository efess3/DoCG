using UnityEngine;

public class EnemySpliter : MonoBehaviour
{
    [Header("Split Settings")]
    [Tooltip("Czas w sekundach do podziału przeciwnika")]
    public float timeToSplit = 3f;
    [Tooltip("Prefab mniejszego przeciwnika, który się pojawi")]
    public GameObject smallerEnemyPrefab;
    [Tooltip("Ile mniejszych przeciwników ma się pojawić")]
    public int spawnCount = 2;

    [Header("Pulse Effect")]
    [Tooltip("Referencja do SpriteRenderer, który będzie pulsował")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Kolor do którego dąży puls (np. czerwony ostrzegawczy)")]
    public Color pulseColor = Color.red;

    private Color originalColor;
    private float timer;
    private float pulsePhase;

    void Start()
    {
        // Jeśli nie przypisano w inspektorze, spróbuj znaleźć na tym samym obiekcie
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
        timer += Time.deltaTime;
        float progress = timer / timeToSplit;

        // Efekt pulsowania
        if (spriteRenderer != null)
        {
            // Przyspieszanie częstotliwości pulsu wraz ze zbliżaniem się do czasu podziału
            float currentFrequency = Mathf.Lerp(5f, 40f, progress);
            pulsePhase += currentFrequency * Time.deltaTime;

            // Wartość od 0 do 1
            float pulse = (Mathf.Sin(pulsePhase) + 1f) / 2f;
            
            spriteRenderer.color = Color.Lerp(originalColor, pulseColor, pulse);
        }

        // Kiedy czas minie, wywołaj podział
        if (timer >= timeToSplit)
        {
            Split();
        }
    }

    void Split()
    {
        if (smallerEnemyPrefab != null)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // Drobne przesunięcie, żeby nie zrespiły się idealnie w tym samym miejscu
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Vector3 spawnPos = transform.position + (Vector3)randomOffset;

                Instantiate(smallerEnemyPrefab, spawnPos, Quaternion.identity);
            }
        }

        // Zniszczenie obecnego przeciwnika
        Destroy(gameObject);
    }
}
