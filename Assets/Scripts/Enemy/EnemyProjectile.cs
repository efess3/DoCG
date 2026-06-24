using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 4f;
    public float damage = 1f;
    public Transform spinObject;
    public float spinSpeed = 360f;

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, 6f);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;

        transform.position += (Vector3)direction * speed * Time.deltaTime;

        if (spinObject != null)
            spinObject.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(Mathf.RoundToInt(damage));

            Destroy(gameObject);
        }
    }
}
