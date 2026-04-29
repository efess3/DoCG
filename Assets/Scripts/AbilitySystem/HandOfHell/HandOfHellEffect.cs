using UnityEngine;

public class HandOfHellEffect : AbilityEffectBase
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            other.GetComponent<MobHealth>()?.TakeDamage(damage);
        }
    }

    // do podpięcia jako Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}