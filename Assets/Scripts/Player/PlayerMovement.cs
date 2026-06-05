using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Health playerHealth;

    private Vector2 movement;

    public bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<Health>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimationAndFlip();
    }

    void HandleInput()
    {
        // global block (game + death)
        if (GameManager.instance != null && !GameManager.instance.isGameActive)
        {
            movement = Vector2.zero;
            return;
        }

        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            movement = Vector2.zero;
            return;
        }

        // movement lock
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }

        // INPUT
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        animator.SetBool("isRunning", movement.magnitude > 0);
    }

    void HandleAnimationAndFlip()
    {
        if (movement.x > 0)
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }

    // =========================
    // PUBLIC API (dla skilli)
    // =========================

    public void LockMovement(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(LockRoutine(duration));
    }

    private IEnumerator LockRoutine(float duration)
    {
        canMove = false;

        rb.linearVelocity = Vector2.zero;
        movement = Vector2.zero;

        yield return new WaitForSeconds(duration);

        canMove = true;
    }

    public void IncreaseMovementSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public float MoveSpeed => moveSpeed;

    public bool IsMoving => canMove && movement.sqrMagnitude > 0.001f;
}
