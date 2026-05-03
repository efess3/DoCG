using UnityEngine;

public class MobHealth : MonoBehaviour
{
    private const string SmallEnemyHitSoundPath = "Sounds/Hit/Ling";
    private const string BossHitSoundPath = "Sounds/Hit/Boss";

    public int maxHealth = 5;
    public int currentHealth;
    public GameObject expCrystalPrefab;

    [Tooltip("Ile krysztalow wypadnie po zabiciu Bossa")]
    public int bossCrystalDropCount = 15;

    private AudioClip[] hitSounds;

    void Start()
    {
        currentHealth = maxHealth;

        switch (tag)
        {
            case "Enemy":
                hitSounds = LoadHitSounds(SmallEnemyHitSoundPath);
                break;
            case "Boss":
                hitSounds = LoadHitSounds(BossHitSoundPath);
                break;
            default:
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlayRandomHitSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private static AudioClip[] LoadHitSounds(string path)
    {
        return Resources.LoadAll<AudioClip>(path);
    }

    private void PlayRandomHitSound()
    {
        if (hitSounds == null || hitSounds.Length == 0) return;

        AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
        AudioSource.PlayClipAtPoint(clip, transform.position, 0.75f);
    }

    void Die()
    {
        if (expCrystalPrefab != null)
        {
            int crystalsToDrop = gameObject.CompareTag("Boss") ? bossCrystalDropCount : 1;

            for (int i = 0; i < crystalsToDrop; i++)
            {
                Vector3 dropPos = transform.position;

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
