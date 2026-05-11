using UnityEngine;

public class ExpCrystal : MonoBehaviour
{
    public int expValue = 1;

    Transform player;

    public float moveSpeed = 8f;

    bool movingToPlayer = false;

    // Magnet radius passed by PlayerMagnet when this crystal enters range.
    // Used as a speed multiplier so bigger pickup range = faster pickup.
    private float magnetRadiusFactor = 1f;
    
    // Zapamiętuje zasięg magnesu, aby przestać lecieć jeśli gracz ucieknie
    private float currentMagnetRadius = 5f;

    // The default (base) magnet radius — speed is unchanged at this value.
    private const float BaseMagnetRadius = 5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// Called by PlayerMagnet when this crystal enters the pickup range.
    /// </summary>
    /// <param name="currentMagnetRadius">
    /// The current magnet radius — used to scale movement speed.
    /// Bigger radius → faster pickup for the same distance.
    /// </param>
    public void StartMoving(float currentMagnetRadius)
    {
        this.currentMagnetRadius = currentMagnetRadius;
        
        if (!movingToPlayer)
        {
            // Only set the factor once — the first magnet detection determines the speed.
            magnetRadiusFactor = currentMagnetRadius / BaseMagnetRadius;
        }
        movingToPlayer = true;
    }

    void Update()
    {
        if (player == null) return;

        if (movingToPlayer)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            // Zatrzymujemy kryształ, jeśli gracz mu uciekł (odległość > zasięg magnesu)
            if (distance > currentMagnetRadius)
            {
                movingToPlayer = false;
                return;
            }

            // Speed formula:
            //   base:    moveSpeed / distance       (closer = faster)
            //   scaled:  * magnetRadiusFactor        (bigger radius = faster overall)
            float currentSpeed = (moveSpeed * magnetRadiusFactor) / Mathf.Max(distance, 0.5f);

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                currentSpeed * Time.deltaTime
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLevel level = other.GetComponent<PlayerLevel>();

            if (level != null)
            {
                level.AddXP(expValue);
            }

            Destroy(gameObject);
        }
    }
}