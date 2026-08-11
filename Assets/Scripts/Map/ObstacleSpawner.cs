using System.Collections.Generic;
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
    
    [Tooltip("Promień 'radaru' sprawdzającego, czy nie nakładamy się na inny collider")]
    public float checkObstacleRadius = 1.5f;

    public void OnChunkSpawned(Vector2 chunkCenter, float chunkSize, Transform chunkParent)
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (Random.value > obstacleChance) return;

        int count = Random.Range(minPerChunk, maxPerChunk + 1);
        float half = chunkSize * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomPosition(chunkCenter, half);
            
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            
            Instabs(prefab, pos, chunkParent);
        }
    }

    private void Instabs(GameObject prefab, Vector2 pos, Transform chunkParent)
    {
        Instantiate(prefab, pos, Quaternion.identity, chunkParent);
    }

    private Vector2 GetRandomPosition(Vector2 center, float half)
    {
        for (int attempt = 0; attempt < 40; attempt++)
        {
            Vector2 candidate = new Vector2(
                center.x + Random.Range(-half, half),
                center.y + Random.Range(-half, half)
            );

            if (Vector2.Distance(candidate, center) < clearRadius)
                continue;

            Collider2D hitCollider = Physics2D.OverlapCircle(candidate, checkObstacleRadius);

            if (hitCollider == null)
            {
                return candidate;
            }
        }

        Vector2 dir = Random.insideUnitCircle.normalized;
        return center + dir * (half * 0.8f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkObstacleRadius);
    }
}