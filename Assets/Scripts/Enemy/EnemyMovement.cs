using UnityEngine;

/*
 Prosta AI:
 przeciwnik cały czas podąża za graczem
*/

public class EnemyMovement : MonoBehaviour
{
    // prędkość przeciwnika
    public float speed = 2f;

    // referencja do gracza
    private Transform player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                Debug.LogError("damage taken");
                playerHealth.TakeDamage(1); // np. 1 dmg
            }
        }
    }

    void Start()
    {
        // znajdujemy gracza w scenie
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;

        if (player == null) return;


        // kierunek do gracza
        Vector2 direction = (player.position - transform.position).normalized;

        // ruch w stronę gracza
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // obrót (obija sprite w osi X w zależności od kierunku)
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}