using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    [Tooltip("Prefab chunka zawierający układ Tile'ów i ewentualne obiekty")]
    public GameObject chunkPrefab;
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
        if (player == null || chunkPrefab == null) return;

        UpdateChunks();
    }

    void UpdateChunks()
    {
        // Określenie na jakim gridzie (chunku) aktualnie znajduje się gracz
        int currentChunkX = Mathf.RoundToInt(player.position.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(player.position.y / chunkSize);

        chunksToKeep.Clear();

        // Pętla tworząca siatkę chunków wokół gracza
        for (int xOffset = -chunkVisibilityRadius; xOffset <= chunkVisibilityRadius; xOffset++)
        {
            for (int yOffset = -chunkVisibilityRadius; yOffset <= chunkVisibilityRadius; yOffset++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunkX + xOffset, currentChunkY + yOffset);
                chunksToKeep.Add(chunkCoord);

                // Jeśli dany chunk (koordynaty) jeszcze nie istnieje, to go instancjujemy
                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Vector2 spawnPosition = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);
                    GameObject newChunk = Instantiate(chunkPrefab, spawnPosition, Quaternion.identity);
                    newChunk.transform.SetParent(this.transform); // Opcjonalnie: uporządkowanie hierarchii
                    activeChunks.Add(chunkCoord, newChunk);
                    obstacleSpawner?.OnChunkSpawned(spawnPosition, chunkSize);
                }
            }
        }

        // Usunięcie starych chunków, które wypadły poza zakres widoczności
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();
        foreach (var chunk in activeChunks)
        {
            if (!chunksToKeep.Contains(chunk.Key))
            {
                Destroy(chunk.Value);
                chunksToRemove.Add(chunk.Key);
            }
        }

        // Wyczyszczenie słownika po usunięciu z niego elementów (unikamy błędów modyfikacji kolekcji w pętli)
        foreach (var coord in chunksToRemove)
        {
            activeChunks.Remove(coord);
        }
    }
}
