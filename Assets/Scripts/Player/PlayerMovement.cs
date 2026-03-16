using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive)
        {
            movement = Vector2.zero;
            return;
        }

        // odczyt inputu
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        Vector2 newPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    public void IncreaseMovementSpeed(float amount)
    {
        moveSpeed += amount;
    }
}