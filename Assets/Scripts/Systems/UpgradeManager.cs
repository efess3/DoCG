using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI Elements")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeTexts;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    public void ShowUpgrades()
    {
        Time.timeScale = 0f; // Pauza gry

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        // Ustawienie 3 losowych ulepszeń dla każdego przycisku
        for (int i = 0; i < 3; i++)
        {
            if (i >= upgradeButtons.Length || i >= upgradeTexts.Length) break; // Bezpieczeństwo

            int upgradeType = Random.Range(0, 4); // 0, 1, 2, 3
            int buttonIndex = i; // lokalna kopia do delegatu

            // Usunięcie poprzednich listenerów, aby uniknąć błędów
            upgradeButtons[buttonIndex].onClick.RemoveAllListeners();

            switch (upgradeType)
            {
                case 0:
                    upgradeTexts[buttonIndex].text = "Zwiększ maksymalne zdrowie\n(+5)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(0));
                    break;
                case 1:
                    upgradeTexts[buttonIndex].text = "Zwiększ szybkość ataku\n(-0.1s)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(1));
                    break;
                case 2:
                    upgradeTexts[buttonIndex].text = "Zwiększ szybkość poruszania się\n(+1)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(2));
                    break;
                case 3:
                    upgradeTexts[buttonIndex].text = "Zwiększ zasięg magnesu\n(+1.5)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(3));
                    break;
            }
        }
    }

    private void ApplyUpgrade(int upgradeType)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            switch (upgradeType)
            {
                case 0:
                    Health health = player.GetComponent<Health>();
                    if (health != null) health.IncreaseMaxHealth(5);
                    break;
                case 1:
                    AutoShooter shooter = player.GetComponentInChildren<AutoShooter>();
                    if (shooter == null) shooter = player.GetComponent<AutoShooter>();
                    if (shooter != null) shooter.IncreaseAttackSpeed(0.1f);
                    break;
                case 2:
                    PlayerMovement movement = player.GetComponent<PlayerMovement>();
                    if (movement != null) movement.IncreaseMovementSpeed(1f);
                    break;
                case 3:
                    PlayerMagnet magnet = player.GetComponent<PlayerMagnet>();
                    if (magnet != null) magnet.IncreaseMagnetRadius(1.5f);
                    break;
            }
        }

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f; // Wznowienie gry po wyborze
    }
}
