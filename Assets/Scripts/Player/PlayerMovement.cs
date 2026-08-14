using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
        // Global block (game active check)
        if (GameManager.instance != null && !GameManager.instance.isGameActive)
        {
            movement = Vector2.zero;
            return;
        }

        // Death check
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            movement = Vector2.zero;
            return;
        }

        // Movement lock (e.g., during skills/knockback)
        if (!canMove)
        {
            movement = Vector2.zero;
            if (animator != null) animator.SetBool("isRunning", false);
            return;
        }

        // Input gathering
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        if (animator != null)
        {
            animator.SetBool("isRunning", movement.sqrMagnitude > 0.001f);
        }
    }

    void HandleAnimationAndFlip()
    {
        if (spriteRenderer == null) return;

        if (movement.x > 0)
            spriteRenderer.flipX = false;
        else if (movement.x < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        // Stop velocity immediately if movement is disabled, dead, or game is paused
        if (!canMove || 
           (playerHealth != null && playerHealth.currentHealth <= 0) || 
           (GameManager.instance != null && !GameManager.instance.isGameActive))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Physics movement
        rb.linearVelocity = movement * moveSpeed;
    }

    // =========================
    // PUBLIC API (for skills/upgrades)
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