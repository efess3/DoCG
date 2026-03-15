using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject startPanel; // przeciągnij tutaj Panel z Hierarchy
    public GameObject startButton;

    public GameObject healthBar;
    public GameObject expBar;

    public void StartGame()
    {
        if (startPanel != null && startButton != null)
        {
            startPanel.SetActive(false); // ukrywa panel
            startButton.SetActive(false);
            healthBar.SetActive(true);
            expBar.SetActive(true);
        }
        else
            Debug.LogError("Start Panel nie jest przypisany!");

        // Dalszy start gry, np. włączenie AI / spawnera
        GameManager.instance.StartGame(); // jeśli masz GameManager
    }
}