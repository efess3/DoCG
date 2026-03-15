using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject startButton;

    public GameObject healthBar;
    public GameObject expBar;

    public void StartGame()
    {
        if (startPanel != null && startButton != null)
        {
            startPanel.SetActive(false);
            startButton.SetActive(false);
            healthBar.SetActive(true);
            expBar.SetActive(true);
        }
        else
            Debug.LogError("Start Panel nie jest przypisany!");

        GameManager.instance.StartGame();
    }
}