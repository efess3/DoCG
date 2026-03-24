using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;

    public int currentXP = 0;

    public int xpToNextLevel = 10;

    public Slider slider;

    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        if (HealthSystem.Instance != null)
        {

            HealthSystem.Instance.maxManaPoint = xpToNextLevel;
            HealthSystem.Instance.manaPoint = currentXP;
            
            HealthSystem.Instance.UpdateGraphics();
        }
    }
    public void AddXP(int amount)
    {
        currentXP += amount;
        HealthSystem.Instance.RestoreMana(amount);

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
            HealthSystem.Instance.manaPoint = 0f;
            HealthSystem.Instance.UpdateGraphics();
        }
    }

    void LevelUp()
    {
        level++;

        currentXP -= xpToNextLevel;

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);

        slider.maxValue = xpToNextLevel;

        Debug.Log("LEVEL UP: " + level);

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.ShowUpgrades();
        }
    }
}