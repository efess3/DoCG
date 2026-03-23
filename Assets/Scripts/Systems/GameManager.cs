using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    [Header("Stats")]
    public float gameTime = 0f;
    public int killCount = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isGameActive = true;
    }

    void Update()
    {
        if (isGameActive)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void AddKill()
    {
        killCount++;
    }
}