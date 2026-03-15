using UnityEngine;

/*
 System spawnujący przeciwników wokół gracza
*/

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnRate = 2f;

    public float spawnDistance = 10f;

    private Transform player;

    float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;

        timer += Time.deltaTime;

        if(timer >= spawnRate)
        {
            SpawnEnemy();
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        // losowy kierunek
        Vector2 direction = Random.insideUnitCircle.normalized;

        // pozycja wokół gracza
        Vector2 spawnPos = (Vector2)player.position + direction * spawnDistance;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}