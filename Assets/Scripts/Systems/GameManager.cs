using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    [Header("Stats")]
    public float gameTime = 0f;
    public int killCount = 0;
    public GameObject GameOverPanel;

    private string sessionStartTime;
    private string mapName;
    private int mapID;

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

            Scene map = SceneManager.GetActiveScene();
            string mapNumber = map.name.Replace("GameScene_Map", "");

            if (!int.TryParse(mapNumber, out mapID))
            {
                Debug.LogError($"Nie można odczytać ID mapy ze sceny: {map.name}");
                mapID = 0;
            }
            LeaderboardManager.instance.AddEntry(playerLevel, killCount, gameTime, sessionStartTime, mapID);
        }

        GameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
