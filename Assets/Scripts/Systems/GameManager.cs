using UnityEngine;

/*
 Zarządza stanem gry
*/

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isGameActive = false;

    void Awake()
    {
        instance = this;
    }

    public void StartGame() {
        isGameActive = true;
    }
}