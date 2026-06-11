using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;
    [Range(0f, 1f)]
    public float obstacleChance = 0.3f;
    public int minPerChunk = 1;
    public int maxPerChunk = 4;
    [Tooltip("Promień wokół środka chunka wolny od przeszkód (gdzie gracz może się spawnować)")]
    public float clearRadius = 3f;

    public void OnChunkSpawned(Vector2 chunkCenter, float chunkSize)
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (Random.value > obstacleChance) return;

        int count = Random.Range(minPerChunk, maxPerChunk + 1);
        float half = chunkSize * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomPosition(chunkCenter, half);
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Instantiate(prefab, pos, Quaternion.identity, transform);
        }
    }

    private Vector2 GetRandomPosition(Vector2 center, float half)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 candidate = new Vector2(
                center.x + Random.Range(-half, half),
                center.y + Random.Range(-half, half)
            );

            if (Vector2.Distance(candidate, center) >= clearRadius)
                return candidate;
        }

        // fallback: krawędź chunka
        Vector2 dir = Random.insideUnitCircle.normalized;
        return center + dir * (half * 0.8f);
    }
}
