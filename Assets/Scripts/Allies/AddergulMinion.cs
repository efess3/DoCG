using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AddergulMinion : MonoBehaviour
{
    private const float StatMultiplier = 0.5f;
    private const float RepathInterval = 0.45f;
    private const float ReachThreshold = 0.15f;
    private const float MoveRadiusMin = 1.25f;
    private const float MoveRadiusMax = 2.75f;
    private const float ReturnDistance = 3.5f;
    private const float IdleFollowDistance = 1.75f;

    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private AutoShooter playerShooter;
    private SpriteRenderer spriteRenderer;
    private Color bulletTint = Color.white;

    private Vector3 currentTarget;
    private float repathTimer;
    private float fireTimer;
    private bool hasTarget;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }

    public void Initialize(
        Transform owner,
        PlayerMovement movement,
        AutoShooter shooter,
        Color tint)
    {
        playerTransform = owner;
        playerMovement = movement;
        playerShooter = shooter;
        bulletTint = tint;

        repathTimer = Random.Range(0f, RepathInterval);
        fireTimer = Random.Range(0f, GetFireInterval());
        PickNewTarget();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdateMovement();
        UpdateAttacks();
    }

    private void UpdateMovement()
    {
        repathTimer -= Time.deltaTime;

        bool playerIsMoving = playerMovement != null && playerMovement.IsMoving;
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (playerIsMoving)
        {
            if (ShouldPickNewMovingTarget())
            {
                PickNewTarget();
            }
        }
        else if (ShouldReturnToPlayer(distanceToPlayer))
        {
            currentTarget = playerTransform.position + (Vector3)GetRandomOffset(0.75f, IdleFollowDistance);
            hasTarget = true;
        }

        if (!hasTarget)
        {
            return;
        }

        Vector3 previousPosition = transform.position;
        float moveStep = GetMoveSpeed() * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveStep);

        UpdateFacing(transform.position - previousPosition);

        bool reachedTarget = Vector2.Distance(transform.position, currentTarget) <= ReachThreshold;
        if (reachedTarget && (!playerIsMoving || distanceToPlayer <= IdleFollowDistance))
        {
            hasTarget = false;
        }
    }

    private void UpdateAttacks()
    {
        if (playerShooter == null || playerShooter.BulletPrefab == null)
        {
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f)
        {
            return;
        }

        if (!TryFindNearestEnemy(out Transform target))
        {
            return;
        }

        Shoot(target);
        fireTimer = GetFireInterval();
    }

    private bool TryFindNearestEnemy(out Transform target)
    {
        MobHealth[] mobs = FindObjectsOfType<MobHealth>();
        if (mobs.Length == 0)
        {
            target = null;
            return false;
        }

        float maxDistance = GetAttackRange();
        float closestDistance = maxDistance;
        target = null;

        foreach (MobHealth mob in mobs)
        {
            if (mob == null) continue;

            float distance = Vector2.Distance(transform.position, mob.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = mob.transform;
            }
        }

        return target != null;
    }

    private void Shoot(Transform target)
    {
        GameObject bulletObject = Instantiate(playerShooter.BulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Destroy(bulletObject);
            return;
        }

        bullet.IncreaseSpeed(playerShooter.BulletSpeedMultiplier * StatMultiplier);
        bullet.SetDamage((bullet.damage + playerShooter.BulletDamageBonus) * StatMultiplier);
        bullet.SetScaleMultiplier(playerShooter.BulletSizeMultiplier * StatMultiplier);
        bullet.SetTint(bulletTint);
        bullet.SetTarget(target);
    }

    private float GetMoveSpeed()
    {
        return playerMovement != null ? playerMovement.MoveSpeed : 2f;
    }

    private float GetAttackRange()
    {
        return playerShooter != null ? Mathf.Max(1f, playerShooter.AttackRange * StatMultiplier) : 1f;
    }

    private float GetFireInterval()
    {
        if (playerShooter == null)
        {
            return 1f;
        }

        return Mathf.Max(0.05f, playerShooter.FireInterval / StatMultiplier);
    }

    private void PickNewTarget()
    {
        currentTarget = playerTransform.position + (Vector3)GetRandomOffset(MoveRadiusMin, MoveRadiusMax);
        repathTimer = RepathInterval;
        hasTarget = true;
    }

    private bool ShouldPickNewMovingTarget()
    {
        return !hasTarget
            || repathTimer <= 0f
            || Vector2.Distance(transform.position, currentTarget) <= ReachThreshold;
    }

    private static bool ShouldReturnToPlayer(float distanceToPlayer)
    {
        return distanceToPlayer > ReturnDistance;
    }

    private void UpdateFacing(Vector3 moveDelta)
    {
        if (moveDelta.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDelta.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private static Vector2 GetRandomOffset(float minRadius, float maxRadius)
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        if (direction == Vector2.zero)
        {
            direction = Vector2.right;
        }

        return direction * Random.Range(minRadius, maxRadius);
    }
}
