using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Settings")]
    public bool isPlayer = false;
    [Header("I-Frames (Nieśmiertelność)")]
    public float invincibilityDuration = 0.5f;
    private float invincibilityTimer;
    
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (isPlayer && HealthSystem.Instance != null)
        {

            HealthSystem.Instance.maxHitPoint = maxHealth;
            HealthSystem.Instance.hitPoint = currentHealth;
            
            HealthSystem.Instance.UpdateGraphics();
        }
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invincibilityTimer > 0) return;

        currentHealth -= (float)damage;
        invincibilityTimer = invincibilityDuration;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else if (isPlayer)
        {
            if(animator != null) animator.SetTrigger("GetHit");
            
            if (HealthSystem.Instance != null)
            {
                HealthSystem.Instance.hitPoint = currentHealth;
                HealthSystem.Instance.UpdateGraphics();
            }
        }
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount;

        if (isPlayer && HealthSystem.Instance != null)
        {
            HealthSystem.Instance.maxHitPoint = maxHealth;
            HealthSystem.Instance.hitPoint = currentHealth;
            HealthSystem.Instance.UpdateGraphics();
        }
    }
    void Die()
    {
        isDead = true;

        if (isPlayer)
        {
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Death");
            Invoke(nameof(TriggerGameOver), 1f);
        }
        else
        {
            animator.SetTrigger("Death");
            Destroy(gameObject, 1f);
        }
    }

    void TriggerGameOver()
    {
        GameManager.instance.GameOver();
    }
}