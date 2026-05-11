using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float range = 10f;
    public float extraBulletSpeed = 0f;

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

        if (extraBulletSpeed > 0f)
            bulletScript.IncreaseSpeed(extraBulletSpeed);

        PlayAttackSound();
        bulletScript.SetTarget(target);
    }

    private void PlayAttackSound()
    {
        if (attackSounds == null || attackSounds.Length == 0) return;

        AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];
        // Use SFXManager so that volume respects the SFX slider setting
        SFXManager.Play(attackAudioSource, clip);
    }

    public void IncreaseBulletSpeed(float amount)
    {
        extraBulletSpeed += amount;
    }

    public void IncreaseAttackSpeed(float amount)
    {
        fireRate -= amount;

        // Zabezpieczenie, żeby gra strzelała poprawnie - nie chcemy ujemnych ani 0 wartśoci
        if (fireRate < 0.1f)
            fireRate = 0.1f;
    }

    public void IncreaseAttackRange(float amount)
    {
        range += amount;
    }
}
