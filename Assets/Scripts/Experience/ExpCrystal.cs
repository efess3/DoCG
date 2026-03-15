using UnityEngine;

public class ExpCrystal : MonoBehaviour
{
    public int expValue = 1;

    Transform player;

    public float pickupRadius = 3f;
    public float moveSpeed = 8f;

    bool movingToPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartMoving()
    {
        movingToPlayer = true;
    }

    void Update()
    {
        if (player == null) return;

        if (movingToPlayer)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            float currentSpeed = moveSpeed / Mathf.Max(distance, 0.5f);

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