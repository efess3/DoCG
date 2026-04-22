using UnityEngine;

public class FireBreathAbility : AbilityBase
{
    protected override void Activate(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        Debug.Log("Spawning ability");

        GameObject effect = Instantiate(
            data.effectPrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log(effect);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effect.transform.rotation = Quaternion.Euler(0, 0, angle);

        var effectScript = effect.GetComponent<AbilityEffectBase>();
        effectScript?.Init(data.damage);

        Debug.Log("Spawning ability");

    }
}