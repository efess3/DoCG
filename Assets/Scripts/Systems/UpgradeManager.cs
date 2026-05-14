using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public enum UpgradeType
{
    Health,
    AttackSpeed,
    AttackPower,
    Speed,
    Radius,
    Cooldowns
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
    private const float HeartDropChancePerUpgrade = 0.05f;
    private const float MaxHeartDropChance = 0.50f;
    private const float AttackSpeedMultiplierPerUpgrade = 1.25f;
    private const float SpeedIncreasePerUpgrade = 1f;
    private const float InvincibilityDurationIncreasePerUpgrade = 0.1f;
    private const float AbilityRadiusIncreasePerUpgrade = 0.10f;
    private const float AttackAndMagnetRangeIncreasePerUpgrade = 0.25f;

    public static UpgradeManager instance;

    [Header("UI Elements")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public Image[] upgradeIcons;
    public TextMeshProUGUI[] upgradeTitles;
    public TextMeshProUGUI[] upgradeDescriptions;

    [Header("Upgrade Database")]
    public Sprite heartPickupSprite;
    public List<UpgradeData> allUpgrades;

    private float heartDropChanceBonus;

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
                case UpgradeType.Health:
                    Health health = player.GetComponent<Health>();
                    if (health != null) health.IncreaseMaxHealth(2f);
                    IncreaseHeartDropChance(HeartDropChancePerUpgrade);
                    break;
                case UpgradeType.AttackSpeed:
                    AutoShooter shooter = FindAutoShooter(player);
                    if (shooter != null) 
                    {
                        shooter.IncreaseAttackSpeed(AttackSpeedMultiplierPerUpgrade);
                        shooter.IncreaseBulletSpeed(1.2f);
                    }
                    break;
                case UpgradeType.AttackPower:
                    AutoShooter powerShooter = FindAutoShooter(player);
                    if (powerShooter != null)
                    {
                        powerShooter.IncreaseBulletDamage(0.5f);
                        powerShooter.IncreaseBulletSize(1.2f);
                    }
                    break;
                case UpgradeType.Speed:
                    PlayerMovement movement = player.GetComponent<PlayerMovement>();
                    if (movement != null) movement.IncreaseMovementSpeed(SpeedIncreasePerUpgrade);

                    Health speedHealth = player.GetComponent<Health>();
                    if (speedHealth != null) speedHealth.IncreaseInvincibilityDuration(InvincibilityDurationIncreasePerUpgrade);
                    break;
                case UpgradeType.Radius:
                    foreach (AbilityBase ability in GetPlayerAbilities(player))
                    {
                        ability.IncreaseAbilityRadius(AbilityRadiusIncreasePerUpgrade);
                    }

                    PlayerMagnet magnet = player.GetComponent<PlayerMagnet>();
                    if (magnet != null) magnet.IncreaseMagnetRadius(AttackAndMagnetRangeIncreasePerUpgrade);

                    AutoShooter radiusShooter = FindAutoShooter(player);
                    if (radiusShooter != null) radiusShooter.IncreaseAttackRange(AttackAndMagnetRangeIncreasePerUpgrade);
                    break;
                case UpgradeType.Cooldowns:
                    foreach (AbilityBase ability in GetPlayerAbilities(player))
                    {
                        ability.ReduceCooldowns(0.10f);
                    }
                    break;
            }
        }

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public float GetHeartDropChance()
    {
        return Mathf.Min(heartDropChanceBonus, MaxHeartDropChance);
    }

    public Sprite GetUpgradeIcon(UpgradeType type)
    {
        UpgradeData upgrade = allUpgrades.FirstOrDefault(data => data.type == type);
        return upgrade.icon;
    }

    public Sprite GetHeartPickupSprite()
    {
        return heartPickupSprite;
    }

    private void IncreaseHeartDropChance(float amount)
    {
        heartDropChanceBonus = Mathf.Min(heartDropChanceBonus + amount, MaxHeartDropChance);
    }

    private static AutoShooter FindAutoShooter(GameObject player)
    {
        AutoShooter shooter = player.GetComponentInChildren<AutoShooter>();
        if (shooter == null)
            shooter = player.GetComponent<AutoShooter>();

        return shooter;
    }

    private static AbilityBase[] GetPlayerAbilities(GameObject player)
    {
        return player.GetComponentsInChildren<AbilityBase>(true);
    }
}

