using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
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
        Time.timeScale = 0f;

        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        List<int> availableUpgrades = Enumerable.Range(0, 6).ToList();
        for (int i = 0; i < 3; i++)
        {
            if (i >= upgradeButtons.Length || i >= upgradeTexts.Length) break;

            // Losowanie bez powtórzeń
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            int upgradeType = availableUpgrades[randomIndex];
            availableUpgrades.RemoveAt(randomIndex);

            int buttonIndex = i;

            upgradeButtons[buttonIndex].onClick.RemoveAllListeners();

            switch (upgradeType)
            {
                case 0:
                    upgradeTexts[buttonIndex].text = "MAX HP\n(+5)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(0));
                    break;
                case 1:
                    upgradeTexts[buttonIndex].text = "ATTACK SPEED\n(-0.1s)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(1));
                    break;
                case 2:
                    upgradeTexts[buttonIndex].text = "MOVEMENT SPEED\n(+1)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(2));
                    break;
                case 3:
                    upgradeTexts[buttonIndex].text = "MAGNET RANGE\n(+1.5)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(3));
                    break;
                case 4:
                    upgradeTexts[buttonIndex].text = "ATTACK RANGE\n(+1)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(4));
                    break;
                case 5:
                    upgradeTexts[buttonIndex].text = "BULLET SPEED\n(+1)";
                    upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(5));
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
                case 4:
                    AutoShooter range = player.GetComponentInChildren<AutoShooter>();
                    if (range != null) range.IncreaseAttackRange(1f);
                    break;
                case 5:
                    AutoShooter autoShooter = player.GetComponentInChildren<AutoShooter>();
                    if (autoShooter == null) autoShooter = player.GetComponent<AutoShooter>();
                    if (autoShooter != null) autoShooter.IncreaseBulletSpeed(1f);
                    break;
            }
        }

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}
