using UnityEngine;

public class FireBreathEffect : AbilityEffectBase
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
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