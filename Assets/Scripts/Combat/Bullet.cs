using UnityEngine;

/*
 Pocisk który leci do celu
*/

public class Bullet : MonoBehaviour
{
    // prędkość pocisku
    public float speed = 5f;

    // obrażenia
    public float damage = 1f;

    // cel
    private Transform target;

    // Zapamiętany, ostatni kierunek lotu
    private Vector2 lastDirection;
    private Vector3 baseScale;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        baseScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Niszczymy pocisk z automatu po 5 sekundach lotu, 
        // by nie leciały w nieskończoność i nie zmniejszały pamięci (optymalizacja)
        Destroy(gameObject, 5f);
    }

    /*
     ustawienie celu pocisku
    */
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        // Obliczamy wejściowy kierunek od razu, po przypisaniu celu
        if (target != null)
        {
            lastDirection = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Aktualizujemy kierunek ciągle w stronę celu tak długo, jak cel istnieje (zachowanie namierzające)
            lastDirection = (target.position - transform.position).normalized;
        }

        // Jeśli kierunek jest przypisany, lecimy
        if (lastDirection != Vector2.zero)
        {
            // ruch pocisku w oparciu o zapamiętany kierunek
            transform.position += (Vector3)lastDirection * speed * Time.deltaTime;

            // obrót w stronę, w którą leci pocisk
            float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // W rzadkim przypadku stworzenia bez celu
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy czy trafiliśmy przeciwnika
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            MobHealth health = other.GetComponent<MobHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void IncreaseSpeed(float amount)
    {
        speed *= amount;
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    public void SetScaleMultiplier(float multiplier)
    {
        transform.localScale = new Vector3(baseScale.x * multiplier, baseScale.y * multiplier, baseScale.z);
    }

    public void SetTint(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
