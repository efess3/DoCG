using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // patrzy w prawo
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // patrzy w lewo
        }
        
        if (GameManager.instance != null && !GameManager.instance.isGameActive)
        {
            movement = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }

        // input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        bool isMoving = movement.magnitude > 0;
        Debug.Log("isRunning: " + (movement.magnitude > 0));
        animator.SetBool("isRunning", isMoving);

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