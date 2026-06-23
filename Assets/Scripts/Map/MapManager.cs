using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [Tooltip("Lista prefabów chunków – gra wylosuje jeden z nich przy tworzeniu nowego terenu")]
    public GameObject[] chunkPrefabs; 
    
    [Tooltip("Rozmiar jednego chunka w jednostkach Unity (np. 20)")]
    public float chunkSize = 20f;
    [Tooltip("Zasięg widoczności chunków (1 = 3x3 grid, 2 = 5x5 grid wokół gracza)")]
    public int chunkVisibilityRadius = 1;

    [Header("References")]
    public Transform player;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> chunksToKeep = new List<Vector2Int>();

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null || chunkPrefabs == null || chunkPrefabs.Length == 0) return;

        UpdateChunks();
    }

    void UpdateChunks()
    {
        int currentChunkX = Mathf.RoundToInt(player.position.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(player.position.y / chunkSize);

        chunksToKeep.Clear();

        for (int xOffset = -chunkVisibilityRadius; xOffset <= chunkVisibilityRadius; xOffset++)
        {
            for (int yOffset = -chunkVisibilityRadius; yOffset <= chunkVisibilityRadius; yOffset++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunkX + xOffset, currentChunkY + yOffset);
                chunksToKeep.Add(chunkCoord);

                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Vector2 spawnPosition = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);
                    
                    int randomIndex = Random.Range(0, chunkPrefabs.Length);
                    GameObject selectedPrefab = chunkPrefabs[randomIndex];

                    GameObject newChunk = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
                    newChunk.transform.SetParent(this.transform); 
                    
                    activeChunks.Add(chunkCoord, newChunk);
                    obstacleSpawner?.OnChunkSpawned(spawnPosition, chunkSize, newChunk.transform);
                }
            }
        }

        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in activeChunks)
        {
            if (!chunksToKeep.Contains(chunk.Key))
            {
                Destroy(chunk.Value);
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in chunksToRemove)
        {
            activeChunks.Remove(coord);
        }
    }
}