using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public enum UpgradeType
{
    MaxHP,
    AttackSpeed,
    MovementSpeed,
    MagnetRange,
    AttackRange,
    BulletSpeed
}

[System.Serializable]
public struct UpgradeData
{
    public UpgradeType type;
    public string title;
    [TextArea]
    public string description;
    public Sprite icon;
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("UI Elements")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public Image[] upgradeIcons;
    public TextMeshProUGUI[] upgradeTitles;
    public TextMeshProUGUI[] upgradeDescriptions;

    [Header("Upgrade Database")]
    public List<UpgradeData> allUpgrades;

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

        if (allUpgrades == null || allUpgrades.Count == 0)
        {
            Debug.LogWarning("No upgrades defined in UpgradeManager!");
            return;
        }

        // Randomize indices from available upgrades
        List<int> availableIndices = Enumerable.Range(0, allUpgrades.Count).ToList();
        
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (availableIndices.Count == 0) break;

            // Losowanie bez powtórzeń
            int randomIndex = Random.Range(0, availableIndices.Count);
            int upgradeIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);

            UpgradeData data = allUpgrades[upgradeIndex];
            int buttonIndex = i;

            upgradeButtons[buttonIndex].onClick.RemoveAllListeners();
            
            // Set UI elements
            if (i < upgradeTitles.Length) upgradeTitles[buttonIndex].text = data.title;
            if (i < upgradeDescriptions.Length) upgradeDescriptions[buttonIndex].text = data.description;
            if (i < upgradeIcons.Length) upgradeIcons[buttonIndex].sprite = data.icon;

            upgradeButtons[buttonIndex].onClick.AddListener(() => ApplyUpgrade(data.type));
        }
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            switch (type)
            {
                case UpgradeType.MaxHP:
                    Health health = player.GetComponent<Health>();
                    if (health != null) health.IncreaseMaxHealth(5);
                    break;
                case UpgradeType.AttackSpeed:
                    AutoShooter shooter = player.GetComponentInChildren<AutoShooter>();
                    if (shooter == null) shooter = player.GetComponent<AutoShooter>();
                    if (shooter != null) shooter.IncreaseAttackSpeed(0.1f);
                    break;
                case UpgradeType.MovementSpeed:
                    PlayerMovement movement = player.GetComponent<PlayerMovement>();
                    if (movement != null) movement.IncreaseMovementSpeed(1f);
                    break;
                case UpgradeType.MagnetRange:
                    PlayerMagnet magnet = player.GetComponent<PlayerMagnet>();
                    if (magnet != null) magnet.IncreaseMagnetRadius(1.5f);
                    break;
                case UpgradeType.AttackRange:
                    AutoShooter range = player.GetComponentInChildren<AutoShooter>();
                    if (range != null) range.IncreaseAttackRange(1f);
                    break;
                case UpgradeType.BulletSpeed:
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

