using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry
{
    public GameObject enemyPrefab;
    [Min(0f)]
    public float probability = 1f;
    [Min(0f)]
    [Tooltip("Czas gry (w sekundach) po jakim ten przeciwnik zaczyna się pojawiać")]
    public float minSpawnTime = 0f;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<EnemySpawnEntry> enemyTypes;
    public GameObject bossPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Minimalny dystans pojawiania się przeciwnika od gracza")]
    public float minSpawnDistance = 25f;
    [Tooltip("Maksymalny dystans pojawiania się przeciwnika od gracza")]
    public float maxSpawnDistance = 30f;
    public int maxNumOfEnemies = 50;

    [Header("Time Scaling")]
    public float baseSpawnRate = 2f;
    public float minimumSpawnRate = 0.5f;

    [Header("Stat Scaling")]
    [Tooltip("O ile procent (0–1) wzrasta HP przeciwników za każdą minutę gry")]
    public float healthScalingPerMinute = 0.3f;
    [Tooltip("O ile procent (0–1) wzrastają obrażenia przeciwników za każdą minutę gry")]
    public float damageScalingPerMinute = 0.2f;

    [Header("Waves & Bosses")]
    [Tooltip("Co ile sekund pojawia się fala wrogów")]
    public float waveInterval = 60f;
    [Tooltip("Ile wrogów na falę")]
    public int enemiesPerWave = 10;

    [Tooltip("Co ile sekund respi się boss (domyślnie 180s = 3 minuty)")]
    public float bossInterval = 180f;
    [Tooltip("Ile bossów na start")]
    public int initialBossCount = 1;
    [Tooltip("Co ile fal bossów zwiększamy ich liczbę")]
    public int bossIncreaseInterval = 2;

    [Header("Late-Game Boss")]
    [Tooltip("Silniejszy boss pojawiający się w późnej fazie gry")]
    public GameObject lateBossPrefab;
    [Tooltip("Po ilu sekundach zaczyna się pojawiać silniejszy boss")]
    public float lateBossStartTime = 300f;

    [Header("Map Difficulty")]
    [Tooltip("Mnożnik bazowego HP wrogów (1 = normalne, 2 = podwójne)")]
    public float healthMultiplier = 1f;
    [Tooltip("Mnożnik bazowych obrażeń wrogów")]
    public float damageMultiplier = 1f;
    [Tooltip("Mnożnik czasu między spawnami (<1 = szybciej, np. 0.75)")]
    public float spawnRateMultiplier = 1f;
    [Header("Limits")]
    [Min(1)]
    public int maxActiveEnemies = 100;

    private readonly HashSet<GameObject> activeEnemies = new();
    private Transform player;
    private float timer;
    private float waveTimer;
    private float bossTimer;
    private int bossWavesPassed = 0;
    private int numOfEnemies = 0;

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

        float currentSpawnRate = Mathf.Max(minimumSpawnRate, baseSpawnRate * spawnRateMultiplier - (GameManager.instance.gameTime / 120f));

        if (timer >= currentSpawnRate)
        {
            SpawnEntity(GetRandomEnemyPrefab());
            numOfEnemies++;
            timer = 0;
        }

        if (waveTimer >= waveInterval)
        {
            SpawnWave(enemiesPerWave);
            waveTimer = 0;
        }

        if (bossTimer >= bossInterval && bossPrefab != null)
        {
            int currentBossCount = initialBossCount + (bossWavesPassed / bossIncreaseInterval);

            GameObject bossToSpawn = (lateBossPrefab != null && GameManager.instance.gameTime >= lateBossStartTime)
                ? lateBossPrefab
                : bossPrefab;

            for (int i = 0; i < currentBossCount; i++)
            {
                SpawnEntity(bossToSpawn, false);
            }

            bossWavesPassed++;
            bossTimer = 0;
        }
    }

    void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEntity(GetRandomEnemyPrefab());
        }
    }

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyTypes == null || enemyTypes.Count == 0) return null;

        float gameTime = GameManager.instance != null ? GameManager.instance.gameTime : 0f;

        float totalProbability = 0f;
        foreach (var entry in enemyTypes)
        {
            if (gameTime >= entry.minSpawnTime)
                totalProbability += entry.probability;
        }

        if (totalProbability <= 0f) return enemyTypes[0].enemyPrefab;

        float randomValue = Random.Range(0f, totalProbability);
        float currentSum = 0f;

        foreach (var entry in enemyTypes)
        {
            if (gameTime < entry.minSpawnTime) continue;
            currentSum += entry.probability;
            if (randomValue <= currentSum)
                return entry.enemyPrefab;
        }

        return enemyTypes[0].enemyPrefab;
    }

    void SpawnEntity(GameObject prefabToSpawn, bool countTowardsLimit = true)
    {
        activeEnemies.RemoveWhere(enemy => enemy == null);

        if (prefabToSpawn == null)
            return;

        if (countTowardsLimit && activeEnemies.Count >= maxActiveEnemies)
            return;

        Vector2 direction = Random.insideUnitCircle.normalized;
        float spawnDist = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector2 spawnPos = (Vector2)player.position + direction * spawnDist;

        GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (countTowardsLimit)
            activeEnemies.Add(spawned);

        ApplyScaling(spawned);
    }

    void ApplyScaling(GameObject enemy)
    {
        if (GameManager.instance == null) return;

        float minutes = GameManager.instance.gameTime / 60f;
        float healthMult = healthMultiplier * (1f + minutes * healthScalingPerMinute);
        float damageMult = damageMultiplier * (1f + minutes * damageScalingPerMinute);

        MobHealth health = enemy.GetComponent<MobHealth>();
        if (health != null)
            health.maxHealth *= healthMult;

        EnemyMovement melee = enemy.GetComponent<EnemyMovement>();
        if (melee != null)
            melee.contactDamage *= damageMult;

        EnemyRangedMovement ranged = enemy.GetComponent<EnemyRangedMovement>();
        if (ranged != null)
            ranged.projectileDamage *= damageMult;
    }
}
