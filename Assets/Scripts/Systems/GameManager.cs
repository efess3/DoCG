using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    [Header("Stats")]
    public float gameTime = 0f;
    public int killCount = 0;
    public GameObject GameOverPanel;

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


    public void GameOver()
    {
        isGameActive = false;

        GameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}