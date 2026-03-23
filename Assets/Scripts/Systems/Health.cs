using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public bool isPlayer = false;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
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

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }
}