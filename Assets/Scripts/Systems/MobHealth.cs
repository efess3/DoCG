using UnityEngine;


public class MobHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public GameObject expCrystalPrefab;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (expCrystalPrefab != null)
        {
            Instantiate(expCrystalPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}