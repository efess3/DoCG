using UnityEngine;

[RequireComponent(typeof(MobHealth))]
public class EnemySpliter : MonoBehaviour
{
    [Header("Split Settings")]
    [Tooltip("Prefab mniejszego przeciwnika, który się pojawi")]
    public GameObject smallerEnemyPrefab;
    [Tooltip("Ile mniejszych przeciwników ma się pojawić")]
    public int spawnCount = 2;

    private MobHealth mobHealth;

    void Awake()
    {
        mobHealth = GetComponent<MobHealth>();
    }

    void OnEnable()
    {
        if (mobHealth != null)
        {
            mobHealth.OnMobDeath += Split;
        }
    }

    void OnDisable()
    {
        if (mobHealth != null)
        {
            mobHealth.OnMobDeath -= Split;
        }
    }

    void Split()
    {
        if (smallerEnemyPrefab != null)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // Drobne przesunięcie, żeby nie zrespiły się idealnie w tym samym miejscu
                Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
                Vector3 spawnPos = transform.position + (Vector3)randomOffset;

                Instantiate(smallerEnemyPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
