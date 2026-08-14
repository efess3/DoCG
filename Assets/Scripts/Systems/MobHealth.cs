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
    public float lifeDuration = 300f;


    public System.Action OnMobDeath;

    [Tooltip("Ile krysztalow wypadnie po zabiciu")]
    public int crystalDropCount = 1;

    private AudioClip[] hitSounds;
    private bool isDead;
    Animator animator;
    private float timeWithoutDamage;

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

    void Update()
{
    if (isDead || lifeDuration <= 0f)
        return;

    timeWithoutDamage += Time.deltaTime;

    if (timeWithoutDamage >= lifeDuration)
    {
        Destroy(gameObject);
    }
}

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        timeWithoutDamage = 0f;

        // 15% chance for a critical hit (deals 2x damage)
        bool isCrit = Random.value < 0.15f;
        float finalDamage = isCrit ? damage * 2f : damage;

        currentHealth -= finalDamage;

        // Show damage numbers if enabled in settings
        if (GameSettingsManager.ShowDamageNumbers && DamageNumberManager.Instance != null)
        {
            DamageNumberManager.Instance.Show(finalDamage, transform.position, isCrit);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger("OnDamage");
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
        if (isDead) return;
        isDead = true;

        PlayRandomHitSound();
        OnMobDeath?.Invoke();

        if (expCrystalPrefab != null)
        {
            MapManager mapManager = FindFirstObjectByType<MapManager>();

            for (int i = 0; i < crystalDropCount; i++)
            {
                Vector3 dropPos = transform.position;

                if (crystalDropCount > 1)
                {
                    dropPos += new Vector3(
                        Random.Range(-1.5f, 1.5f),
                        Random.Range(-1.5f, 1.5f),
                        0f
                    );
                }

                Transform chunkParent = mapManager != null
                    ? mapManager.GetChunkParent(dropPos)
                    : null;

                GameObject crystal = Instantiate(expCrystalPrefab, dropPos, Quaternion.identity);

                if (chunkParent != null)
                    crystal.transform.SetParent(chunkParent, true);
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
