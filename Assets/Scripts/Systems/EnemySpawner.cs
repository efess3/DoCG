using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Minimalny dystans pojawiania się przeciwnika od gracza")]
    public float minSpawnDistance = 25f;
    [Tooltip("Maksymalny dystans pojawiania się przeciwnika od gracza")]
    public float maxSpawnDistance = 30f;
    
    [Header("Time Scaling")]
    public float baseSpawnRate = 2f;
    public float minimumSpawnRate = 0.5f;

    [Header("Waves & Bosses")]
    [Tooltip("Co ile sekund pojawia się fala wrogów")]
    public float waveInterval = 60f;
    [Tooltip("Ile wrogów na falę")]
    public int enemiesPerWave = 10;
    
    [Tooltip("Co ile sekund respi się boss (domyślnie 180s = 3 minuty)")]
    public float bossInterval = 180f;

    private Transform player;
    private float timer;
    private float waveTimer;
    private float bossTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (GameManager.instance != null && !GameManager.instance.isGameActive) return;
        if (player == null) return;

        timer += Time.deltaTime;
        waveTimer += Time.deltaTime;
        bossTimer += Time.deltaTime;

        // Skalowanie trudności z czasem
        float currentSpawnRate = Mathf.Max(minimumSpawnRate, baseSpawnRate - (GameManager.instance.gameTime / 120f));

        if (timer >= currentSpawnRate)
        {
            SpawnEntity(enemyPrefab);
            timer = 0;
        }

        // Fale (Waves)
        if (waveTimer >= waveInterval)
        {
            SpawnWave(enemiesPerWave);
            waveTimer = 0;
        }

        // Boss (co określoną liczbę sekund)
        if (bossTimer >= bossInterval && bossPrefab != null)
        {
            SpawnEntity(bossPrefab);
            bossTimer = 0;
        }
    }

    void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEntity(enemyPrefab);
        }
    }

    void SpawnEntity(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null) return;

        // Losowy kierunek
        Vector2 direction = Random.insideUnitCircle.normalized;

        // Zasięg respu z dala od gracza
        float spawnDist = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector2 spawnPos = (Vector2)player.position + direction * spawnDist;

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }
}