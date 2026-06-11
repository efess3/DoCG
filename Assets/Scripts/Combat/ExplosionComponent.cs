using System.Collections;
using UnityEngine;

public class ExplosionComponent : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("Ilość obrażeń zadawanych przez wybuch")]
    public int damage = 10;
    [Tooltip("Promień wybuchu")]
    public float radius = 3f;
    [Tooltip("Opóźnienie przed zadaniem obrażeń (np. by zsynchronizować z animacją)")]
    public float explosionDelay = 0f;
    [Tooltip("Opóźnienie przed usunięciem obiektu wybuchu (po zadaniu obrażeń)")]
    public float destroyDelay = 2f;

    void Start()
    {
        Explode();
    }

    /// <summary>
    /// Wykonuje wybuch zadający obrażenia w promieniu zdefiniowanym w skrypcie.
    /// </summary>
    public void Explode()
    {
        // Skalujemy promień w oparciu o skalę obiektu
        float worldRadius = radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        StartCoroutine(ExplosionRoutine(damage, worldRadius, explosionDelay));
    }

    private IEnumerator ExplosionRoutine(int damage, float worldRadius, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        // Trigger screen shake on explosion
        if (CameraFollow.Instance != null)
        {
            CameraFollow.Instance.TriggerShake(0.25f, 0.3f);
        }

        // Znajdujemy wszystkie collidery w zasięgu (używamy overlap bez potrzeby posiadania collidera na tym obiekcie)
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, worldRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Zadajemy obrażenia graczowi (Health.cs)
            Health health = hitCollider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Zadajemy obrażenia przeciwnikom (MobHealth.cs)
            MobHealth mobHealth = hitCollider.GetComponent<MobHealth>();
            if (mobHealth != null)
            {
                mobHealth.TakeDamage(damage);
            }
        }

        // Samoczynne usunięcie obiektu po zakończeniu logiki wybuchu
        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float worldRadius = radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        Gizmos.DrawWireSphere(transform.position, worldRadius);
    }
}
