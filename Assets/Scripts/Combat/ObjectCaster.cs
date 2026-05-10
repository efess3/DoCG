using UnityEngine;
using System.Collections;

public class ObjectCaster : MonoBehaviour
{
    [Header("Caster Settings")]
    [Tooltip("Lista prefabów, które mogą zostać zrespione")]
    public GameObject[] prefabsToCast;
    [Tooltip("Co ile sekund ma następować zrespienie obiektu")]
    public float castInterval = 5f;
    [Tooltip("Jak długo ma trwać pulsowanie ostrzegawcze przed zrespieniem")]
    public float pulseDuration = 2f;

    [Header("Pulse Effect")]
    [Tooltip("Referencja do SpriteRenderer, który będzie pulsował")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Kolor do którego dąży puls (np. czerwony ostrzegawczy)")]
    public Color pulseColor = Color.red;

    private Color originalColor;
    private float pulsePhase;

    void Start()
    {
        // Próba znalezienia SpriteRenderer, jeśli nie został przypisany
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Rozpoczęcie pętli rzucania obiektów
        StartCoroutine(CasterLoop());
    }

    IEnumerator CasterLoop()
    {
        while (true)
        {
            // Czekamy na kolejny interwał minus czas pulsowania
            float waitTime = castInterval - pulseDuration;
            if (waitTime > 0)
            {
                yield return new WaitForSeconds(waitTime);
            }

            // Faza pulsowania
            float pulseTimer = 0f;
            pulsePhase = 0f;

            while (pulseTimer < pulseDuration)
            {
                pulseTimer += Time.deltaTime;
                float progress = pulseTimer / pulseDuration;

                if (spriteRenderer != null)
                {
                    // Przyspieszanie częstotliwości pulsu (tak jak w bombie)
                    float currentFrequency = Mathf.Lerp(5f, 40f, progress);
                    pulsePhase += currentFrequency * Time.deltaTime;

                    float pulse = (Mathf.Sin(pulsePhase) + 1f) / 2f;
                    spriteRenderer.color = Color.Lerp(originalColor, pulseColor, pulse);
                }
                yield return null;
            }

            // Zrespienie obiektu
            Cast();

            // Powrót do oryginalnego koloru po zrespieniu
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            // Zabezpieczenie przed nieskończoną pętlą o zerowym czasie, jeśli interwały są źle ustawione
            if (castInterval <= 0)
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    void Cast()
    {
        if (prefabsToCast == null || prefabsToCast.Length == 0) return;

        // Losujemy jeden z przypisanych prefabów
        GameObject prefab = prefabsToCast[Random.Range(0, prefabsToCast.Length)];
        
        if (prefab != null)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }
    }
}
