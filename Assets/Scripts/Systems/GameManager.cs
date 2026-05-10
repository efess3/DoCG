using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    [Header("Stats")]
    public float gameTime = 0f;
    public int killCount = 0;
    public GameObject GameOverPanel;

    private string sessionStartTime;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        isGameActive = true;
        Debug.Log("Game started");
        sessionStartTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        BackgroundMusicManager.PlayGameplayMusic();
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

        // Save to leaderboard
        if (LeaderboardManager.instance != null)
        {
            int playerLevel = 1;
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PlayerLevel pl = player.GetComponent<PlayerLevel>();
                if (pl != null) playerLevel = pl.level;
            }

            LeaderboardManager.instance.AddEntry(playerLevel, killCount, gameTime, sessionStartTime);
        }

        GameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
