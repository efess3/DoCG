using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public AbilityData data;

    [Header("Level Requirement")]
    [Tooltip("Player must reach this level to unlock this ability.")]
    public int requiredLevel = 1;
    public bool isUnlocked = false;

    protected float lastUseTime;
    protected bool isAiming;
    protected float cooldownMultiplier = 1f;
    protected float abilityRadiusMultiplier = 0.5f;

    protected PlayerMovement playerMovement;
    protected GameObject previewInstance;

    // =========================
    // INIT
    // =========================

    protected virtual void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        RefreshUnlockState();
    }

    /// <summary>
    /// Call this whenever the player levels up to re-evaluate whether this ability should unlock.
    /// </summary>
    public void RefreshUnlockState()
    {
        PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
        if (playerLevel != null)
            isUnlocked = playerLevel.level >= requiredLevel;
    }

    // =========================
    // AIM START
    // =========================

    public virtual void StartAiming()
    {
        if (!CanUse()) return;

        isAiming = true;

        CreatePreview();
    }

    // =========================
    // AIM UPDATE
    // =========================

    public virtual void UpdateAiming(Vector2 targetPos)
    {
        if (!isAiming) return;

        Vector2 dir = targetPos - (Vector2)transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        previewInstance.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // =========================
    // RELEASE / CAST
    // =========================

    public virtual void Release(Vector2 targetPos)
    {
        if (!isAiming) return;

        isAiming = false;

        DestroyPreview();

        if (!CanUse()) return;

        lastUseTime = Time.time;

        Activate(targetPos);

        LockPlayer();
    }

    // =========================
    // CORE ABILITY LOGIC
    // =========================

    protected abstract void Activate(Vector2 targetPos);

    // =========================
    // COOLDOWN
    // =========================

    protected bool CanUse()
    {
        if (!isUnlocked) return false;
        return Time.time >= lastUseTime + GetCurrentCooldown();
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, (lastUseTime + GetCurrentCooldown()) - Time.time);
    }

    public float GetCooldownNormalized()
    {
        float currentCooldown = GetCurrentCooldown();
        if (currentCooldown <= 0) return 0;
        return GetCooldownRemaining() / currentCooldown;
    }

    // =========================
    // PREVIEW
    // =========================

    protected virtual void CreatePreview()
    {
        if (data.previewPrefab == null) return;
        
        previewInstance = Instantiate(data.previewPrefab, transform);
        previewInstance.transform.localPosition = data.previewOffset;
        ApplyAbilityRadius(previewInstance.transform);
    }

    protected virtual void DestroyPreview()
    {
        if (previewInstance != null)
            Destroy(previewInstance);
    }

    // =========================
    // PLAYER LOCK
    // =========================

    protected void LockPlayer()
    {
        if (playerMovement == null) return;

        playerMovement.LockMovement(data.castTimeLock);
    }

    public void ReduceCooldowns(float percentage)
    {
        cooldownMultiplier *= Mathf.Max(0.05f, 1f - percentage);
    }

    public void IncreaseAbilityRadius(float percentage)
    {
        float upgradeMultiplier = 1f + percentage;
        abilityRadiusMultiplier *= upgradeMultiplier;

        if (previewInstance != null)
            ApplyAbilityRadius(previewInstance.transform, upgradeMultiplier);
    }

    protected float GetAbilityRadiusMultiplier()
    {
        return abilityRadiusMultiplier;
    }

    public float CooldownMultiplier => cooldownMultiplier;

    public float AbilityRadiusMultiplier => abilityRadiusMultiplier;

    public float BaseCooldown => data != null ? data.cooldown : 0f;

    public float CurrentCooldown => BaseCooldown * cooldownMultiplier;

    protected void ApplyAbilityRadius(Transform target)
    {
        ApplyAbilityRadius(target, abilityRadiusMultiplier);
    }

    protected void ApplyAbilityRadius(Transform target, float scaleMultiplier)
    {
        if (target == null) return;

        target.localScale = new Vector3(
            target.localScale.x * scaleMultiplier,
            target.localScale.y * scaleMultiplier,
            target.localScale.z
        );
    }

    private float GetCurrentCooldown()
    {
        return data.cooldown * cooldownMultiplier;
    }
}
