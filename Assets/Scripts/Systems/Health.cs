using UnityEngine;

public class Health : MonoBehaviour
{
    private const string TakingDamageSoundPath = "Sounds/Taking_Damage";

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
    private AudioClip[] takingDamageSounds;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        takingDamageSounds = Resources.LoadAll<AudioClip>(TakingDamageSoundPath);

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
        PlayTakingDamageSound();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // Update the health bar UI to show 0 before dying
            if (isPlayer && HealthSystem.Instance != null)
            {
                HealthSystem.Instance.hitPoint = 0;
                HealthSystem.Instance.UpdateGraphics();
            }

            Die();
        }
        else if (isPlayer)
        {
            if (animator != null) animator.SetTrigger("GetHit");
            
            if (CameraFollow.Instance != null)
            {
                CameraFollow.Instance.TriggerShake(0.18f, 0.25f);
            }
            
            if (HealthSystem.Instance != null)
            {
                HealthSystem.Instance.hitPoint = currentHealth;
                HealthSystem.Instance.UpdateGraphics();
            }
        }
    }

    private void PlayTakingDamageSound()
    {
        if (!isPlayer || takingDamageSounds == null || takingDamageSounds.Length == 0) return;

        AudioClip clip = takingDamageSounds[Random.Range(0, takingDamageSounds.Length)];
        AudioSource.PlayClipAtPoint(clip, transform.position, 1.0f);
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

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (isPlayer && HealthSystem.Instance != null)
        {
            HealthSystem.Instance.hitPoint = currentHealth;
            HealthSystem.Instance.UpdateGraphics();
        }
    }

    public void IncreaseInvincibilityDuration(float amount)
    {
        if (amount <= 0f) return;
        invincibilityDuration += amount;
    }

    void Die()
    {
        isDead = true;

        // FIXED: Disable ALL attached colliders (physics + trigger)
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        if (isPlayer)
        {
            if (animator != null) animator.SetTrigger("Death");
            Invoke(nameof(TriggerGameOver), 1f);
        }
        else
        {
            if (animator != null) animator.SetTrigger("Death");
            Destroy(gameObject, 1f);
        }
    }

    void TriggerGameOver()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
    }
}