using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Settings")]
    public bool isPlayer = false;
    
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

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= (float)damage;

        if (isPlayer && HealthSystem.Instance != null)
        {
            HealthSystem.Instance.TakeDamage((float)damage);
        }

        if (currentHealth <= 0)
        {
            Die();
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

        animator.SetTrigger("Death");

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