using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Text;

public enum UpgradeType
{
    Health,
    AttackSpeed,
    AttackPower,
    Speed,
    Radius,
    Cooldowns,
    SummonAddergul
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
    private const float AddergulScaleMultiplier = 0.75f;

    public static UpgradeManager instance;

    [Header("UI Elements")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public Image[] upgradeIcons;
    public TextMeshProUGUI[] upgradeTitles;
    public TextMeshProUGUI[] upgradeDescriptions;

    [Header("Upgrade Database")]
    public Sprite heartPickupSprite;
    public GameObject addergulDarkBluePrefab;
    public GameObject addergulOrangePrefab;
    public GameObject addergulPinkPrefab;
    public GameObject addergulWhitePrefab;
    public List<UpgradeData> allUpgrades;

    private float heartDropChanceBonus;

    private readonly struct AddergulData
    {
        public AddergulData(GameObject prefab, Color bulletTint)
        {
            Prefab = prefab;
            BulletTint = bulletTint;
        }

        public GameObject Prefab { get; }
        public Color BulletTint { get; }
    }

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
                case UpgradeType.SummonAddergul:
                    SummonAddergul(player);
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

    public string GetPauseStatsText()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return "PLAYER STATS\nPlayer not found.";
        }

        Health health = player.GetComponent<Health>();
        AutoShooter shooter = FindAutoShooter(player);
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        PlayerMagnet magnet = player.GetComponent<PlayerMagnet>();
        AbilityBase[] abilities = GetPlayerAbilities(player);
        AbilityBase representativeAbility = abilities.FirstOrDefault();
        Bullet bulletPrefab = shooter != null && shooter.BulletPrefab != null
            ? shooter.BulletPrefab.GetComponent<Bullet>()
            : null;

        float shotsPerSecond = shooter != null && shooter.FireInterval > 0f ? 1f / shooter.FireInterval : 0f;
        float bulletSpeed = bulletPrefab != null && shooter != null
            ? bulletPrefab.speed * shooter.BulletSpeedMultiplier
            : 0f;
        float bulletDamage = bulletPrefab != null && shooter != null
            ? bulletPrefab.damage + shooter.BulletDamageBonus
            : 0f;
        int addergulCount = FindObjectsOfType<AddergulMinion>().Length;

        StringBuilder builder = new StringBuilder(512);
        builder.AppendLine("PLAYER STATS");
        AppendStatLine(builder, "Max HP", FormatValue(health != null ? health.maxHealth : (float?)null));
        AppendStatLine(builder, "Heart drop rate", FormatPercent(GetHeartDropChance()));
        AppendStatLine(builder, "Fire rate", shooter != null ? $"{FormatValue(shotsPerSecond)} shots/s" : "-");
        AppendStatLine(builder, "Bullet speed", FormatValue(bulletSpeed, shooter != null && bulletPrefab != null));
        AppendStatLine(builder, "Damage", FormatValue(bulletDamage, shooter != null && bulletPrefab != null));
        AppendStatLine(builder, "Bullet size", shooter != null ? FormatMultiplier(shooter.BulletSizeMultiplier) : "-");
        AppendStatLine(builder, "Move speed", FormatValue(movement != null ? movement.MoveSpeed : (float?)null));
        AppendStatLine(builder, "Invincibility", health != null ? $"{FormatValue(health.invincibilityDuration)} s" : "-");
        AppendStatLine(builder, "Ability radius", representativeAbility != null ? FormatMultiplier(representativeAbility.AbilityRadiusMultiplier) : "-");
        AppendStatLine(builder, "Pickup range", FormatValue(magnet != null ? magnet.magnetRadius : (float?)null));
        AppendStatLine(builder, "Attack range", FormatValue(shooter != null ? shooter.AttackRange : (float?)null));
        AppendStatLine(builder, "Ability cooldowns", representativeAbility != null ? FormatMultiplier(representativeAbility.CooldownMultiplier) : "-");
        AppendStatLine(builder, "Addergul minions", addergulCount.ToString());
        return builder.ToString().TrimEnd();
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

    private static void AppendStatLine(StringBuilder builder, string label, string value)
    {
        builder.Append(label);
        builder.Append(": ");
        builder.AppendLine(value);
    }

    private static string FormatMultiplier(float value)
    {
        return $"x{value:0.##}";
    }

    private static string FormatPercent(float value)
    {
        return $"{value * 100f:0.#}%";
    }

    private static string FormatValue(float? value, bool hasValue = true)
    {
        return hasValue && value.HasValue ? value.Value.ToString("0.##") : "-";
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

    private void SummonAddergul(GameObject player)
    {
        if (!TryGetAddergulOwner(player, out AutoShooter shooter, out PlayerMovement movement, out SpriteRenderer playerSpriteRenderer))
        {
            Debug.LogWarning("Cannot summon Addergul without PlayerMovement and AutoShooter on the player.");
            return;
        }

        AddergulVariant variant = (AddergulVariant)Random.Range(0, 4);
        AddergulData addergulData = GetAddergulData(variant);
        if (addergulData.Prefab == null)
        {
            Debug.LogWarning($"Missing prefab for summoned Addergul variant: {variant}");
            return;
        }

        GameObject minionObject = CreateAddergulMinionFromTemplate(
            addergulData.Prefab,
            $"Addergul Minion ({variant})",
            player.transform.position + (Vector3)GetAddergulSpawnOffset(),
            player.transform.localScale * AddergulScaleMultiplier);

        if (minionObject == null)
        {
            Debug.LogWarning($"Failed to create summoned Addergul from template: {addergulData.Prefab.name}");
            return;
        }

        SpriteRenderer minionSpriteRenderer = minionObject.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null && minionSpriteRenderer != null)
        {
            ApplyPlayerSorting(playerSpriteRenderer, minionSpriteRenderer);
        }

        AddergulMinion minion = minionObject.GetComponent<AddergulMinion>();
        if (minion == null)
        {
            Debug.LogWarning($"Summoned Addergul prefab is missing {nameof(AddergulMinion)}: {addergulData.Prefab.name}");
            Destroy(minionObject);
            return;
        }

        minion.Initialize(
            player.transform,
            movement,
            shooter,
            addergulData.BulletTint);
    }

    private static GameObject CreateAddergulMinionFromTemplate(GameObject template, string objectName, Vector3 spawnPosition, Vector3 spawnScale)
    {
        if (template == null)
        {
            return null;
        }

        SpriteRenderer templateRenderer = template.GetComponent<SpriteRenderer>();
        if (templateRenderer == null)
        {
            return null;
        }

        GameObject minionObject = Instantiate(template, spawnPosition, Quaternion.identity);
        minionObject.name = objectName;
        minionObject.transform.localScale = spawnScale;

        if (minionObject.GetComponent<AddergulMinion>() == null)
        {
            minionObject.AddComponent<AddergulMinion>();
        }

        return minionObject;
    }

    private static void ApplyPlayerSorting(SpriteRenderer playerSpriteRenderer, SpriteRenderer minionSpriteRenderer)
    {
        minionSpriteRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
        minionSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
    }

    private bool TryGetAddergulOwner(
        GameObject player,
        out AutoShooter shooter,
        out PlayerMovement movement,
        out SpriteRenderer playerSpriteRenderer)
    {
        shooter = FindAutoShooter(player);
        movement = player.GetComponent<PlayerMovement>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();

        return shooter != null && movement != null;
    }

    private AddergulData GetAddergulData(AddergulVariant variant)
    {
        switch (variant)
        {
            case AddergulVariant.DarkBlue:
                return new AddergulData(addergulDarkBluePrefab, new Color(0.36f, 0.67f, 1f, 1f));
            case AddergulVariant.Orange:
                return new AddergulData(addergulOrangePrefab, new Color(1f, 0.55f, 0.18f, 1f));
            case AddergulVariant.Pink:
                return new AddergulData(addergulPinkPrefab, new Color(1f, 0.47f, 0.78f, 1f));
            case AddergulVariant.White:
                return new AddergulData(addergulWhitePrefab, new Color(0.95f, 0.98f, 1f, 1f));
            default:
                return new AddergulData(null, Color.white);
        }
    }

    private static Vector2 GetAddergulSpawnOffset()
    {
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 1.5f;
        return spawnOffset == Vector2.zero ? Vector2.right * 1.5f : spawnOffset;
    }

    private enum AddergulVariant
    {
        DarkBlue,
        Orange,
        Pink,
        White
    }
}

