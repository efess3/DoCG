using UnityEngine;

public class MobHealth : MonoBehaviour
{
    private const string SmallEnemyHitSoundPath = "Sounds/Hit/Ling";
    private const string BossHitSoundPath = "Sounds/Hit/Boss";
    private const float HeartDropOffsetMin = 0.35f;
    private const float HeartDropOffsetMax = 0.8f;

    public float maxHealth = 5f;
    public float currentHealth;
    public GameObject expCrystalPrefab;
    public float heartHealAmount = 1f;

    public System.Action OnMobDeath;

    [Tooltip("Ile krysztalow wypadnie po zabiciu Bossa")]
    public int bossCrystalDropCount = 15;

    private AudioClip[] hitSounds;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        PlayRandomHitSound();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("OnDamage");
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
        OnMobDeath?.Invoke();

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

        TryDropHeart();

        Destroy(gameObject);
    }

    private void TryDropHeart()
    {
        if (UpgradeManager.instance == null) return;

        float heartDropChance = UpgradeManager.instance.GetHeartDropChance();
        if (heartDropChance <= 0f || Random.value > heartDropChance) return;

        HeartPickup.Spawn(GetHeartDropPosition(), heartHealAmount);
    }

    private Vector3 GetHeartDropPosition()
    {
        Vector2 offsetDirection = Random.insideUnitCircle;
        if (offsetDirection == Vector2.zero)
            offsetDirection = Vector2.right;

        float offsetDistance = Random.Range(HeartDropOffsetMin, HeartDropOffsetMax);
        Vector2 offset = offsetDirection.normalized * offsetDistance;
        return transform.position + (Vector3)offset;
    }
}
