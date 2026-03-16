using UnityEngine;

/*
 Zarządza stanem gry
*/

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    [Header("Game Stats")]
    public float gameTime = 0f;
    public int killCount = 0;

    void Awake()
    {
        instance = this;
    }

    public void StartGame() {
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