using UnityEngine;

public class HandOfHellAbility : AbilityBase
{
    protected override void CreatePreview()
    {
        if (data.previewPrefab == null) return;

        previewInstance = Instantiate(data.previewPrefab);
        ApplyAbilityRadius(previewInstance.transform);
    }

    public override void UpdateAiming(Vector2 targetPos)
    {
        if (!isAiming || previewInstance == null) return;

        previewInstance.transform.position = targetPos;
    }

    protected override void Activate(Vector2 targetPos)
    {
        GameObject effect = Instantiate(
            data.effectPrefab,
            targetPos,
            Quaternion.identity
        );
        ApplyAbilityRadius(effect.transform);

        AbilityEffectBase effectScript = effect.GetComponent<AbilityEffectBase>();
        effectScript?.Init(data.damage);
    }
}
