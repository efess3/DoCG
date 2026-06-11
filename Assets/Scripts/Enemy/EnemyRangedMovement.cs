using UnityEngine;

public class EnemyRangedMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    [Tooltip("Dystans, w którym enemy zatrzymuje się i zaczyna strzelać")]
    public float attackRange = 8f;
    [Tooltip("Dystans, w którym enemy ucieka od gracza")]
    public float retreatRange = 4f;
    public float fireRate = 2.5f;
    public GameObject projectilePrefab;
    public float projectileDamage = 1f;

    private Transform player;
    private float fireTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<Animator>()?.SetFloat("Speed", moveSpeed);
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        if (dist > attackRange)
        {
            transform.position += (Vector3)dirToPlayer * moveSpeed * Time.deltaTime;
        }
        else if (dist < retreatRange)
        {
            transform.position -= (Vector3)dirToPlayer * moveSpeed * Time.deltaTime;
        }
        else
        {
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                Shoot(dirToPlayer);
                fireTimer = 0f;
            }
        }

        if (dirToPlayer.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dirToPlayer.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Shoot(Vector2 direction)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.damage = projectileDamage;
            ep.SetDirection(direction);
        }
    }
}
