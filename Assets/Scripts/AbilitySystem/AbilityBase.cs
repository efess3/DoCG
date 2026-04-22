using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public AbilityData data;

    protected float lastUseTime;
    protected bool isAiming;

    protected GameObject previewInstance;

    public virtual void StartAiming()
    {
        if (!CanUse()) return;

        isAiming = true;

        if (data.previewPrefab != null)
            previewInstance = Instantiate(data.previewPrefab);
    }

    public virtual void UpdateAiming(Vector2 targetPos)
    {
        if (!isAiming) return;

        if (previewInstance != null)
            previewInstance.transform.position = targetPos;
    }

    public virtual void Release(Vector2 targetPos)
    {
        if (!isAiming) return;

        isAiming = false;

        if (previewInstance != null)
            Destroy(previewInstance);

        if (!CanUse()) return;

        lastUseTime = Time.time;

        Activate(targetPos);
    }

    protected abstract void Activate(Vector2 targetPos);

    protected bool CanUse()
    {
        return Time.time >= lastUseTime + data.cooldown;
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0, (lastUseTime + data.cooldown) - Time.time);
    }
}