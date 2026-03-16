using UnityEngine;


public class MobHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public GameObject expCrystalPrefab;
    
    [Tooltip("Ile kryształów wypadnie po zabiciu Bossa")]
    public int bossCrystalDropCount = 15;

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
            int crystalsToDrop = gameObject.CompareTag("Boss") ? bossCrystalDropCount : 1;

            for (int i = 0; i < crystalsToDrop; i++)
            {
                Vector3 dropPos = transform.position;
                
                // Jeśli wypada więcej niż 1, rozrzucamy je lekko wokół pokonanego
                if (crystalsToDrop > 1)
                {
                    dropPos += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
                }

                Instantiate(expCrystalPrefab, dropPos, Quaternion.identity);
            }
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill();
        }

        Destroy(gameObject);
    }
}