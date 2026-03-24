using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 10;
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
        SyncXPWithUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.RestoreMana(amount);
        }

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);
        SyncXPWithUI();

        Debug.Log("LEVEL UP: " + level);

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
}