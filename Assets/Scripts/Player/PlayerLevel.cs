using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 5;
    private Animator animator;

    [Header("UI")]
    [Tooltip("Drag a TextMeshPro UI Text here to display the current level.")]
    public TextMeshProUGUI levelText;

    public void Start()
    {
        animator = GetComponent<Animator>();
        xpToNextLevel = GetXPRequiredForLevel(level);
        SyncXPWithUI();
        UpdateLevelText();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.RestoreMana(amount);
        }

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        
        xpToNextLevel = GetXPRequiredForLevel(level);
        SyncXPWithUI();

        Debug.Log("LEVEL UP: " + level);

        UpdateLevelText();

        // Refresh unlock state for all abilities on this GameObject
        foreach (var ability in GetComponents<AbilityBase>())
            ability.RefreshUnlockState();

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.ShowUpgrades();
        }
    }

    // Nowa metoda pomocnicza, która "mówi" wszystkim paskom, jakie są nowe limity
    private void SyncXPWithUI()
    {
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.maxManaPoint = xpToNextLevel;
            HealthSystem.Instance.manaPoint = currentXP;
            HealthSystem.Instance.UpdateGraphics(); 
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "LVL " + level;
    }

    private static int GetXPRequiredForLevel(int currentLevel)
    {
        return Mathf.FloorToInt(Mathf.Pow(currentLevel, 1.5f) + (2 * currentLevel) + 2);
    }
}
