using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    private const float AttackSoundVolumeMultiplier = 0.1f;

    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float range = 10f;
    public float bulletSpeedMultiplier = 1f;
    public float bulletDamageBonus = 0f;
    public float bulletSizeMultiplier = 1f;

    // ── Manual / Auto-aim ─────────────────────────────────────────────────────
    /// <summary>When true, Shoot() aims at manualAimTarget instead of nearest enemy.</summary>
    public bool UseManualAim { get; set; } = false;
    private Vector3 manualAimTarget;

    private AudioClip[] attackSounds;
    private AudioSource attackAudioSource;
    float fireTimer;

    void Awake()
    {
        attackSounds = Resources.LoadAll<AudioClip>("Sounds/Attack");
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.playOnAwake = false;
        attackAudioSource.spatialBlend = 0f;
    }

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    /// <summary>Called by AutoAimController every frame when manual aim is active.</summary>
    public void SetManualAimTarget(Vector3 worldPosition) => manualAimTarget = worldPosition;

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab nie jest przypisany!");
            return;
        }

        if (UseManualAim)
        {
            ShootAtPosition(manualAimTarget);
        }
        else
        {
            ShootAtNearestEnemy();
        }
    }

    // ── Targeting modes ───────────────────────────────────────────────────────

    private void ShootAtNearestEnemy()
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        if (enemies.Length == 0) return;

        EnemyMovement closestEnemy = null;
        float minDistance = range;

        foreach (EnemyMovement enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null) return;

        SpawnBullet(closestEnemy.transform);
    }

    private void ShootAtPosition(Vector3 targetWorld)
    {
        // Create a temporary aim transform at the mouse position
        GameObject aimProxy = new GameObject("_AimProxy");
        aimProxy.transform.position = new Vector3(targetWorld.x, targetWorld.y, transform.position.z);

        SpawnBullet(aimProxy.transform);

        Destroy(aimProxy);
    }

    private void SpawnBullet(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript == null)
        {
            Debug.LogError("Prefab Bullet nie ma skryptu Bullet!");
            return;
        }

        if (bulletSpeedMultiplier > 1f)
            bulletScript.IncreaseSpeed(bulletSpeedMultiplier);

        bulletScript.SetDamage(bulletScript.damage + bulletDamageBonus);
        bulletScript.SetScaleMultiplier(bulletSizeMultiplier);

        PlayAttackSound();
        bulletScript.SetTarget(target);
    }

    private void PlayAttackSound()
    {
        if (attackSounds == null || attackSounds.Length == 0) return;

        AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];
        // Keep attack SFX much quieter while still respecting the global SFX slider.
        SFXManager.Play(attackAudioSource, clip, AttackSoundVolumeMultiplier);
    }

    public GameObject BulletPrefab => bulletPrefab;

    public float FireInterval => fireRate;

    public float AttackRange => range;

    public float BulletSpeedMultiplier => bulletSpeedMultiplier;

    public float BulletDamageBonus => bulletDamageBonus;

    public float BulletSizeMultiplier => bulletSizeMultiplier;

    public void IncreaseBulletSpeed(float amount)
    {
        bulletSpeedMultiplier *= amount;
        if(bulletSpeedMultiplier > 4)
        bulletSpeedMultiplier = 4;
    }

    public void IncreaseAttackSpeed(float amount)
    {
        fireRate /= amount;

        if (fireRate < 0.01f)
            fireRate = 0.01f;
    }

    public void IncreaseAttackRange(float amount)
    {
        range += amount;
    }

    public void IncreaseBulletDamage(float amount)
    {
        bulletDamageBonus += amount;
    }

    public void IncreaseBulletSize(float amount)
    {
        bulletSizeMultiplier *= amount;
        if (bulletSizeMultiplier > 4f)
            bulletSizeMultiplier = 4f;
    }
}
