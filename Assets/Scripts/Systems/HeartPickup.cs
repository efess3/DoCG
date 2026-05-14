using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    private const float VisualScale = 3f;
    [SerializeField] private float healAmount = 1f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float moveSpeed = 8f;

    private Transform player;
    private bool movingToPlayer;
    private float magnetRadiusFactor = 1f;
    private float currentMagnetRadius = 5f;

    private const float BaseMagnetRadius = 5f;

    public static void Spawn(Vector3 position, float healValue)
    {
        Sprite sprite = GetHeartSprite();
        if (sprite == null)
        {
            Debug.LogWarning("Heart sprite is not assigned in UpgradeManager.");
            return;
        }

        GameObject heartObject = new GameObject("HeartPickup");
        heartObject.transform.position = position;
        heartObject.transform.localScale = new Vector3(VisualScale, VisualScale, 1f);

        SpriteRenderer spriteRenderer = heartObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 10;

        Rigidbody2D rigidbody2D = heartObject.GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
            rigidbody2D = heartObject.AddComponent<Rigidbody2D>();

        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.simulated = true;

        CircleCollider2D collider = heartObject.GetComponent<CircleCollider2D>();
        if (collider == null)
            collider = heartObject.AddComponent<CircleCollider2D>();

        collider.isTrigger = true;
        collider.radius = 0.5f;

        HeartPickup pickup = heartObject.AddComponent<HeartPickup>();

        pickup.healAmount = healValue;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;

        Destroy(gameObject, lifetime);
    }

    public void StartMoving(float magnetRadius)
    {
        currentMagnetRadius = magnetRadius;

        if (!movingToPlayer)
            magnetRadiusFactor = magnetRadius / BaseMagnetRadius;

        movingToPlayer = true;
    }

    private void Update()
    {
        if (player == null || !movingToPlayer) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > currentMagnetRadius)
        {
            movingToPlayer = false;
            return;
        }

        float currentSpeed = (moveSpeed * magnetRadiusFactor) / Mathf.Max(distance, 0.5f);
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            currentSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null || !health.isPlayer) return;

        health.Heal(healAmount);
        Destroy(gameObject);
    }

    private static Sprite GetHeartSprite()
    {
        if (UpgradeManager.instance == null) return null;

        return UpgradeManager.instance.GetHeartPickupSprite();
    }
}
