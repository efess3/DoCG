using UnityEngine;

/*
 Pocisk który leci do celu
*/

public class Bullet : MonoBehaviour
{
    // prędkość pocisku
    public float speed = 10f;

    // obrażenia
    public int damage = 1;

    // cel
    private Transform target;

    /*
     ustawienie celu pocisku
    */
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // kierunek do celu
        Vector2 direction = (target.position - transform.position).normalized;

        // ruch pocisku
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // obrót w stronę, w którą leci pocisk
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy czy trafiliśmy przeciwnika
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            MobHealth health = other.GetComponent<MobHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

}