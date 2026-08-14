using UnityEngine;

/*
 Prosta AI:
 Przeciwnik podąża za graczem z wykorzystaniem fizyki (Rigidbody2D)
*/

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float contactDamage = 1f;
    public float damageInterval = 0.5f; // Time in seconds between damage ticks

    private Transform player;
    private Rigidbody2D rb;
    private float nextDamageTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Znajdujemy gracza w scenie
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", speed);
        }
    }

    void FixedUpdate()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;
        if (player == null) return;

        // Kierunek do gracza
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        // Ruch w stronę gracza używając fizyki (nie przenika przez ściany)
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // Obrót w osi X
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
    Health playerHealth = other.GetComponentInParent<Health>();

    if (playerHealth != null && playerHealth.isPlayer &&
        Time.time >= nextDamageTime)
    {
        playerHealth.TakeDamage(Mathf.RoundToInt(contactDamage));
        nextDamageTime = Time.time + damageInterval;
    }
    }
}