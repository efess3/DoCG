using UnityEngine;

/*
 Prosta AI:
 przeciwnik cały czas podąża za graczem
*/

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float contactDamage = 1f;

    private Transform player;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Mathf.RoundToInt(contactDamage));
            }
        }
    }

    void Start()
    {
        // znajdujemy gracza w scenie
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<Animator>().SetFloat("Speed", speed);
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