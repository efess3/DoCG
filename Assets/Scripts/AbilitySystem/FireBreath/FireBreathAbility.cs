using UnityEngine;

public class FireBreathAbility : AbilityBase
{
    protected override void Activate(Vector2 targetPos)
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        previewInstance = Instantiate(data.previewPrefab);
        previewInstance.transform.SetParent(transform);
        previewInstance.transform.localPosition = new Vector3(0, 0.3f, 0);
        Destroy(previewInstance);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        GameObject effect = Instantiate(
            data.effectPrefab,
            transform.position,
            Quaternion.identity
        );
        ApplyAbilityRadius(effect.transform);

        Debug.Log(effect);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effect.transform.rotation = Quaternion.Euler(0, 0, angle);

        var effectScript = effect.GetComponent<AbilityEffectBase>();
        effectScript?.Init(data.damage);

        
    }
}
